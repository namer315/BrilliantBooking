var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "ERPBooking"
}));

app.Run();
