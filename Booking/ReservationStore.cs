using System.Text.Json;

namespace ERPBooking.Booking;

public sealed class ReservationStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly RestaurantCatalog _catalog;
    private readonly ILogger<ReservationStore> _logger;

    public ReservationStore(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        RestaurantCatalog catalog,
        ILogger<ReservationStore> logger)
    {
        var configuredPath = configuration["Booking:DataPath"];
        _filePath = Path.GetFullPath(string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(environment.ContentRootPath, "App_Data", "reservations.json")
            : configuredPath);
        _catalog = catalog;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            return await ReadAllAsync(cancellationToken);
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<ReservationResult> TryCreateAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var reservations = await ReadAllAsync(cancellationToken);
            var restaurant = _catalog.Find(reservation.RestaurantSlug);
            if (restaurant is null ||
                !restaurant.GetSlots(reservation.Date, CapeTownClock.Today).Contains(reservation.Time) ||
                reservation.Date == CapeTownClock.Today &&
                reservation.Time <= CapeTownClock.CurrentTime ||
                reservations
                    .Where(existing =>
                        existing.RestaurantSlug == reservation.RestaurantSlug &&
                        existing.Date == reservation.Date &&
                        existing.Time == reservation.Time)
                    .Sum(existing => existing.Guests) + reservation.Guests > restaurant.CoversPerSlot)
            {
                return ReservationResult.Unavailable;
            }

            var updatedReservations = reservations.Append(reservation).ToArray();
            var directory = Path.GetDirectoryName(_filePath)
                ?? throw new InvalidOperationException("The reservation data path must include a directory.");
            Directory.CreateDirectory(directory);
            var temporaryPath = Path.Combine(directory, $"{Path.GetFileName(_filePath)}.{Guid.NewGuid():N}.tmp");

            try
            {
                await using (var stream = new FileStream(
                    temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await JsonSerializer.SerializeAsync(stream, updatedReservations, JsonOptions, cancellationToken);
                    await stream.FlushAsync(cancellationToken);
                }

                File.Move(temporaryPath, _filePath, overwrite: true);
            }
            catch
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }

                throw;
            }

            _logger.LogInformation(
                "Reservation {ReservationReference} created for restaurant {RestaurantSlug}",
                reservation.Reference,
                reservation.RestaurantSlug);
            return ReservationResult.Confirmed;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<IReadOnlyList<Reservation>> ReadAllAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = new FileStream(
            _filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return await JsonSerializer.DeserializeAsync<Reservation[]>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidDataException($"Reservation data file '{_filePath}' contains invalid data.");
    }
}
