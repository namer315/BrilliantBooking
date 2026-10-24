using Data;
using Data.DAO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("TD");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
builder.Services.AddSingleton(_ => new NHibernateSessionFactory(connectionString));
builder.Services.AddScoped<NHibernateRepository>();
builder.Services.AddScoped<UserDAO>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "ERPBooking"
}));

app.Run();
