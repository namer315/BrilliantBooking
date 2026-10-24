using ERPBooking.Booking;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<RestaurantCatalog>();
builder.Services.AddSingleton<ReservationStore>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "ERPBooking"
}));

app.Run();
