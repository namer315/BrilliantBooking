using Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<NHibernateSessionFactory>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("TD");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
    return new NHibernateSessionFactory(connectionString);
});
builder.Services.AddScoped<NHibernateRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "ERPBooking"
}));

app.Run();
