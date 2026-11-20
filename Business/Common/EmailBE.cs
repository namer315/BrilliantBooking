using Data.DTO.Common;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Business.Common;

internal sealed class EmailConfig
{
    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool EnableSsl { get; set; } = true;
}

internal class EmailBE
{
    private static EmailConfig config = null;
    private const string ConfigFileName = "EmailConfig.json";


    internal static async Task<EmailConfig> LoadConfigAsync()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory , ConfigFileName);

        if (!File.Exists(configPath))
        {
            EmailConfig template = new();
            string templateJson = JsonSerializer.Serialize(template , new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configPath , templateJson);

            throw new FileNotFoundException(
                $"The email configuration file was created at '{configPath}'. Configure it before sending email." ,
                configPath);
        }

        await using FileStream stream = File.OpenRead(configPath);
        return await JsonSerializer.DeserializeAsync<EmailConfig>(stream)
            ?? throw new InvalidOperationException($"The email configuration file '{configPath}' is empty.");
    }

    private static void ValidateConfig(EmailConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.SmtpHost))
        {
            throw new InvalidOperationException("The SMTP host is required in EmailConfig.json.");
        }

        if (config.SmtpPort is < 1 or > 65535)
        {
            throw new InvalidOperationException("The SMTP port in EmailConfig.json must be between 1 and 65535.");
        }

        if (string.IsNullOrWhiteSpace(config.Username))
        {
            throw new InvalidOperationException("The SMTP username is required in EmailConfig.json.");
        }

        if (string.IsNullOrWhiteSpace(config.Password))
        {
            throw new InvalidOperationException("The SMTP password is required in EmailConfig.json.");
        }

        try
        {
            _ = new MailAddress(config.Username);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("The SMTP username in EmailConfig.json must be a valid email address." , exception);
        }
    }

    public static async Task SendAsync(EmailDTO email)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(email.To))
        {
            throw new ArgumentException("The recipient email address is required." , nameof(email));
        }

        try
        {
            _ = new MailAddress(email.To);
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("The recipient email address is invalid." , nameof(email) , exception);
        }

        try
        {

            config ??= await LoadConfigAsync();
            ValidateConfig(config);

            using var client = new SmtpClient(config.SmtpHost , config.SmtpPort)
            {
                Credentials = new NetworkCredential(config.Username , config.Password) ,
                EnableSsl = config.EnableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(config.Username) ,
                Subject = email.Subject ,
                Body = email.Body ,
                IsBodyHtml = email.IsHtml
            };

            message.To.Add(email.To);

            await client.SendMailAsync(message);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Email delivery failed. SMTP host: '{config?.SmtpHost ?? "(unknown)"}', recipient: '{email.To}', error: {exception.Message}");
            config = null;
        }
    }
}
