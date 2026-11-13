using BrilliantBooking.Components;
using BrilliantBooking.Services;
using CommonData.Session;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // In the services section, before builder.Build()
            // Add distributed cache and session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
                options.Cookie.Name = "BrilliantBooking.Session"; // Custom session cookie name
            });

            builder.Services.AddScoped<SessionService>();
            builder.Services.AddScoped<BookingUiStateService>();
            builder.Services.AddScoped<LoadingService>();
            builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();

            //Connection connection = new()
            //{
            //    DataBaseName = "BrilliantBooking" ,
            //    User = "sa" ,
            //    Password = "123456"
            //};
            //await connection.SessionConnect();
            await Connection.DataBaseConnect(new Connection()
            {
                DataBaseName = "BrilliantBooking"
            });
            // update DB

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error" , createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/not-found" , createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            // In the pipeline, after app.UseHttpsRedirection() and before app.MapRazorComponents<App>()
            app.UseSession();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}