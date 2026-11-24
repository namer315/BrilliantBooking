using Data.Extensions;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Operations;
using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Pages.Owner;

public partial class PageDashboard : PageBase
{
    private readonly DashboardFDM _fdm = new();
    //private readonly DashboardSH Search = new();
    private readonly BookingSH Search = new();
    private IList<BookingVO> BookingList = new List<BookingVO>();
    private IList<RestaurantVO> ResturantList = new List<RestaurantVO>();
    private int _selectedVenueIndex;

    private static IReadOnlyList<BookingVO.BookingStatus> BookingStatuses { get; } =
        Enum.GetValues<BookingVO.BookingStatus>();

    private IList<FloorVO> FloorList { get; set; }
    private IList<TableVO> TableList => FloorList is { Count : > 0}? FloorList[0].TableList : new List<TableVO>();
    private IList<ArrivalVO> Arrivals { get; } = new List<ArrivalVO>();

    private int TotalShiftCovers => 42;
    private int ShiftCapacity => 60;
    private int CapacityPercentage => TotalShiftCovers * 100 / ShiftCapacity;
    private int ActiveTableCount => TableList.Count(table => table.Status != TableVO.TableStatus.Available);
    private int ReservedTableCount => TableList.Count(table => table.Status == TableVO.TableStatus.Reserved);
    private string NextArrivalTime => Arrivals.Count == 0 ? "--:--" : Arrivals.Min(arrival => arrival.Time).ToString(@"hh\:mm");

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await beginEdit();
            await dgSearch();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task dgSearch()
    {
        BookingList = await _fdm.GetBookingList(Search);
    }

    private async Task beginEdit()
    {
        ResturantList = await _fdm.GetResturantList();
        Search.Resturant = ResturantList.FirstOrDefault();
        //IList<TableVO> TableList = await _fdm.GetTableListBy(Search.Resturant);
        //Search.Resturant = CreateSampleVenues()[0];

        //var venues = await _fdm.GetOperationsVenuesAsync();
        //ResturantList = venues.Count > 0 ? venues : CreateSampleVenues();
        PopulateSampleArrivals();
    }

    private async Task OnVenueChanged(ChangeEventArgs args)
    {
        if (ResturantList is { Count: > 0 } && Guid.TryParse(args.Value?.ToString() , out Guid resturantId))
        {
            Search.Resturant = ResturantList.FirstOrDefault(x => x.Id == resturantId);
            FloorList = await _fdm.GetFloorListBy(Search.Resturant);
            PopulateSampleArrivals();
            await dgSearch();
        }
    }
    #region Booking Management

    private async Task OnBookingTextChanged(ChangeEventArgs args)
    {
        Search.Text = args.Value?.ToString() ?? string.Empty;
        await RefreshBookingListAsync();
    }

    private async Task OnFromDateChanged(ChangeEventArgs args)
    {
        Search.FromDate = args.Value.ParseDateOnlyOrNull();
        await RefreshBookingListAsync();
    }

    private async Task OnToDateChanged(ChangeEventArgs args)
    {
        Search.ToDate = args.Value.ParseDateOnlyOrNull();
        await RefreshBookingListAsync();
    }

    private async Task ResetBookingFiltersAsync()
    {
        Search.Text = string.Empty;
        Search.FromDate = DateOnly.Date;
        Search.ToDate = null;
        await RefreshBookingListAsync();
    }

    private async Task OnBookingStatusChanged(BookingVO booking, ChangeEventArgs args)
    {
        if (Enum.TryParse<BookingVO.BookingStatus>(args.Value?.ToString(), out var status))
        {
            booking.Status = status;
            BookingList = BookingList.ToList();
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task RefreshBookingListAsync()
    {
        try
        {
            await dgSearch();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion Booking Management


    private async Task RefreshOperationsAsync()
    {
        try
        {
            await dgSearch();
            var venues = await _fdm.GetOperationsVenuesAsync();
            if (venues.Count > 0)
            {
                //ResturantList = venues;
                _selectedVenueIndex = Math.Min(_selectedVenueIndex , ResturantList.Count - 1);
                PopulateSampleArrivals();
            }
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private void ToggleTableStatus(TableVO table)
    {
        table.Status = table.Status switch
        {
            TableVO.TableStatus.Available => TableVO.TableStatus.Selected,
            TableVO.TableStatus.Selected => TableVO.TableStatus.Reserved,
            _ => TableVO.TableStatus.Available
        };
    }

    private void SeatGuest(ArrivalVO arrival)
    {
        arrival.Status = "Seated";
    }

    private static string GetTableClass(TableVO table)
        => table.Status switch
        {
            TableVO.TableStatus.Reserved => "reserved",
            TableVO.TableStatus.Selected => "selected",
            _ => "available"
        };

    private static string GetTableStatus(TableVO table)
        => table.Status switch
        {
            TableVO.TableStatus.Reserved => "RESERVED",
            TableVO.TableStatus.Selected => "SELECTED",
            _ => "AVAILABLE"
        };

    private static string GetTableLabel(TableVO table)
        => $"{table.Name}, {GetTableStatus(table)}, capacity {table.Capacity}";

    private static string GetArrivalClass(ArrivalVO arrival)
        => string.Equals(arrival.Status, "Seated", StringComparison.OrdinalIgnoreCase) ? "arrival-seated" : "arrival-upcoming";

    private void PopulateSampleArrivals()
    {
        Arrivals.Clear();
        foreach (var arrival in new[]
        {
            new ArrivalVO
            {
                Time = new TimeSpan(19, 30, 0), GuestName = "Vance Party", PartySize = 4,
                TableName = "Table 04", Tags = "Gluten-Free", Status = "Seated", Restaurant = Search.Resturant
            },
            new ArrivalVO
            {
                Time = new TimeSpan(19, 45, 0), GuestName = "Dr. A. Smith", PartySize = 2,
                TableName = "Table 12", Tags = "VIP", Status = "Expected", Restaurant = Search.Resturant
            },
            new ArrivalVO
            {
                Time = new TimeSpan(20, 15, 0), GuestName = "Claire Dupont", PartySize = 6,
                TableName = "Patio Booth P1", Tags = "Birthday", Status = "Expected", Restaurant = Search.Resturant
            }
        })
        {
            Arrivals.Add(arrival);
        }
    }

    private static IList<RestaurantVO> CreateSampleVenues()
    {
        return new List<RestaurantVO>
        {
            CreateSampleVenue("Ocean Grill & Raw Bar", "Seafood", 3, 25),
            CreateSampleVenue("Le Petit Manoir", "French", 4, 18),
            CreateSampleVenue("Kuro Omakase", "Japanese", 4, 14),
            CreateSampleVenue("La Trattoria del Sol", "Italian", 2, 20)
        };
    }

    private static RestaurantVO CreateSampleVenue(string name, string cuisine, int priceLevel, int tableCount)
    {
        var venue = new RestaurantVO
        {
            Id = Guid.NewGuid(),
            Name = name,
            Cuisine = cuisine,
            PriceLevel = priceLevel,
            Location = "Cape Town",
            FloorList = new List<FloorVO>()
        };
        var floor = new FloorVO { Id = Guid.NewGuid(), Name = "Main floor", Restaurant = venue };
        floor.TableList = CreateSampleTables(tableCount, floor);
        venue.FloorList.Add(floor);
        return venue;
    }

    private static IList<TableVO> CreateSampleTables(int tableCount, FloorVO floor)
    {
        var tables = new List<TableVO>();
        for (var index = 0; index < Math.Min(tableCount, 8); index++)
        {
            tables.Add(new TableVO
            {
                Id = Guid.NewGuid(),
                Name = $"T-{index + 1:00}",
                Capacity = index % 3 == 0 ? 4 : 2,
                GuestCount = index % 3 == 0 ? 4 : 2,
                Zone = index % 2 == 0 ? "Indoor" : "Terrace",
                Status = index switch
                {
                    1 => TableVO.TableStatus.Reserved,
                    2 => TableVO.TableStatus.Selected,
                    _ => TableVO.TableStatus.Available
                },
                Floor = floor
            });
        }

        return tables;
    }
}