using Data.DTO;
using Data.VO;
using FDM;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BrilliantBooking.Services;

public class GlobalPreProcessor
{
    private GlobalPreProcessorFDM _fdm = new GlobalPreProcessorFDM();
    private const string LoginCredentialsCookieName =
        "BrilliantBooking.LoginCredentials";

    private readonly IDataProtector _loginCredentialsProtector;
    private readonly SessionService _sessionService;
    public GlobalPreProcessor(IDataProtectionProvider dataProtectionProvider, SessionService sessionService)
    {
        _loginCredentialsProtector = dataProtectionProvider.CreateProtector(
            "BrilliantBooking.LoginCredentials.v1");
        _sessionService = sessionService;
    }

    public async Task PreProcessAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path.StartsWith("/_framework" , StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        else if (path.StartsWith("/_content" , StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        if (IsPageRequest(context))
        {
            await ProcessPageAsync(context);
        }
    }

    private bool IsPageRequest(HttpContext context)
    {
        var path = context.Request.Path.Value;

        return HttpMethods.IsGet(context.Request.Method)
            && !string.IsNullOrWhiteSpace(path)
            && !Path.HasExtension(path);
    }

    private async Task ProcessPageAsync(HttpContext context)
    {
        if(!_sessionService.IsPrivateUser)
        {
            if (GetRememberedSignIn(context) is SignInDTO signIn)
            {
                if (await _fdm.SignInAsync(signIn) is UserVO user)
                {
                    await _sessionService.SetOnlineUserSessionAsync(user);
                }
            }
            else
            {

            }
        }
        else
        {
            _sessionService.SendOnlineUserKeyToBackEnd();
        }

    }

    public bool CookieExists(HttpContext context , string cookieName)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Request.Cookies.TryGetValue(cookieName , out var value)
            && !string.IsNullOrWhiteSpace(value);
    }
    public bool HasLoginCredentialsCookie(HttpContext context)
    {
        return CookieExists(context , LoginCredentialsCookieName);
    }

    public SignInDTO GetRememberedSignIn(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.Cookies.TryGetValue(LoginCredentialsCookieName , out var cookieValue)
            || string.IsNullOrWhiteSpace(cookieValue))
        {
            return null;
        }

        try
        {
            var protectedJson = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(cookieValue));
            var signInJson = _loginCredentialsProtector.Unprotect(protectedJson);

            return JsonSerializer.Deserialize<SignInDTO>(signInJson);
        }
        catch (CryptographicException)
        {
            return null;
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
