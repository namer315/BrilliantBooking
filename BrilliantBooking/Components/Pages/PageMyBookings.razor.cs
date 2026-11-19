using BrilliantBooking.Extensions;
using Business;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Restaurant;
using FDM;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class PageMyBookings
{
    private BookingFDM _fdm { get; set; } = new();
    private BookingSH Search { get; set; } = new();

    public IList<BookingVO> BookingList { get; private set; } = new List<BookingVO>();
    public string BookingsTab { get; private set; } = "active";
    public string LookupCode { get; set; } = "";

    //public bool IsCustomer => SessionService.HasValidSessionKeys;

    public IEnumerable<BookingVO> VisibleBookings => BookingList.Where(b =>
        BookingsTab == "active"
            ? b.Status == BookingVO.BookingStatus.Active
            : b.Status != BookingVO.BookingStatus.Active);

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await dgSearch();
        }
        catch(Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task dgSearch()
    {
        if (!IsCustomer) return;
        BookingList = await _fdm.GetBookingBy(Search).ShowProcessing(this);
        //var oceanGrill = new RestaurantVO { Id = Guid.NewGuid(), Name = "Ocean Grill & Raw Bar" };
        //var petitManoir = new RestaurantVO { Id = Guid.NewGuid(), Name = "Le Petit Manoir" };

        //BookingList = new List<BookingVO>()
        //{
        //    new BookingVO { Reference = "#TBL-8942-CPT", Restaurant = oceanGrill, Date = DateOnly.FromDateTime(DateTime.Today), Time = new(19, 30, 0), GuestCount = 4, TableName = "Table 04 (Ocean Window)", Status = BookingVO.BookingStatus.Active },
        //    new BookingVO { Reference = "#TBL-2041-FRN", Restaurant = petitManoir, Date = new(2026, 10, 14), Time = new(20, 0, 0), GuestCount = 2, TableName = "Chef's Counter C1", Status = BookingVO.BookingStatus.Past }
        //};
        //await Task.CompletedTask;
    }

    public void SetBookingsTab(string tab) => BookingsTab = tab;

    public void CancelBooking(BookingVO booking) => booking.Status = BookingVO.BookingStatus.Cancelled;

    public async Task ModifyBooking() =>
        await JS.InvokeVoidAsync("alert", "Modification request sent to host.");

    public void RebookRestaurant(RestaurantVO restaurant) =>
        NavigationManager.NavigateTo($"/App/Restaurant/{restaurant.Id}");

    public string FormatDate(DateOnly date) =>
        date == DateOnly.FromDateTime(DateTime.Today) ? "Today" : date.ToString("d MMM yyyy");

    public string FormatTime(TimeSpan time) =>
        $"{time.Hours:00}:{time.Minutes:00}";

    public async Task LookupBooking()
    {
        var query = LookupCode.Trim();
        if (string.IsNullOrEmpty(query)) return;

        var found = BookingList.FirstOrDefault(b =>
            b.Reference.Equals(query, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrEmpty(b.Customer.Phone) && b.Customer.Phone == query));
        var message = found is not null
            ? $"Reservation Found!\n\nVenue: {found.Restaurant.Name}\nDate: {FormatDate(found.Date)} at {FormatTime(found.FromTime)}\nTable: {found.BookingTable?.Name}"
            : $"No active reservation found matching code: {query}";
        await JS.InvokeVoidAsync("alert", message);
    }
}
