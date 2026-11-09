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
            builder.Services.AddScoped<BookingUiStateService>();

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