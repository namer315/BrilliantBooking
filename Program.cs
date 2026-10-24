using ERPBooking.Booking;
using Data;
using Data.DAO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<RestaurantCatalog>();
builder.Services.AddSingleton<ReservationStore>();

if (builder.Configuration.GetValue<bool>("RestaurantProfiles:UseDatabase"))
{
    var connectionString = builder.Configuration.GetConnectionString("RestaurantProfiles");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    builder.Services.AddSingleton(new NHibernateSessionFactory(connectionString));
    builder.Services.AddSingleton<NHibernateRepository>();
    builder.Services.AddSingleton<RestaurantProfileDAO>();
}

builder.Services.AddSingleton(serviceProvider => new RestaurantProfileService(
    serviceProvider.GetRequiredService<RestaurantCatalog>(),
    serviceProvider.GetService<RestaurantProfileDAO>()));

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "ERPBooking"
}));

app.Run();
