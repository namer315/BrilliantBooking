namespace ERPBooking.Booking;

public sealed record Restaurant(
    string Slug,
    string Name,
    string Cuisine,
    string Area,
    int PriceLevel,
    decimal Rating,
    int ReviewCount,
    string Description,
    string Address,
    string Phone,
    string OpeningHours,
    string MenuSummary,
    string ImageUrl,
    string ImageAlt,
    string Highlight,
    TimeOnly OpensAt,
    TimeOnly ClosesAt,
    int CoversPerSlot,
    IReadOnlyList<DayOfWeek> OpenDays)
{
    public IReadOnlyList<TimeOnly> GetSlots(DateOnly date, DateOnly today)
    {
        if (date < today || !OpenDays.Contains(date.DayOfWeek))
        {
            return [];
        }

        var slots = new List<TimeOnly>();
        for (var time = OpensAt; time < ClosesAt; time = time.AddMinutes(30))
        {
            slots.Add(time);
        }

        return slots;
    }

    public string PriceLabel => new string('R', PriceLevel);
}
