using BrilliantBooking.Extensions;
using Data.VO.Restaurant;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Dialog;

public partial class RestaurantDialog : ComponentBase, IAsyncDisposable
{
    [Parameter, EditorRequired]
    public RestaurantVO Restaurant { get; set; } = null!;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback<RestaurantVO> OnReserve { get; set; }

    private readonly List<StaticReview> _userReviews = [];
    private readonly List<string> _imageSources = [];
    private CancellationTokenSource? _carouselCancellation;
    private Task? _carouselTask;
    private bool _showReviewForm;
    private string _newReviewName = "";
    private string _newReviewComment = "";
    private int _newReviewRating = 5;
    private int _activeImageIndex;
    private string _tab = "overview";

    private static readonly StaticReview[] _reviews =
    [
        new("Sarah M.", "2 days ago", 5, "Outstanding service and the signature dishes were beautifully presented. The window table is worth requesting."),
        new("James L.", "1 week ago", 4, "Excellent wine pairing and a relaxed atmosphere. Parking was easy with valet."),
        new("Amélie D.", "2 weeks ago", 5, "A memorable evening for our anniversary. The chef's counter experience is a must.")
    ];

    private static readonly RatingMetric[] _ratingMetrics =
    [
        new("Food Quality", "bg-apple-green", 96, 4.9m),
        new("Service", "bg-apple-blue", 94, 4.8m),
        new("Ambience", "bg-apple-purple", 98, 5.0m)
    ];

    private string CurrentImageSrc => _imageSources[_activeImageIndex];

    private string PriceText => new('$', Math.Clamp(Restaurant.PriceLevel, 1, 4));

    private string MapsUrl => $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(Restaurant.Address)}";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var imageSources = BuildImageSources();
        var imagesChanged = !_imageSources.SequenceEqual(imageSources);

        if (imagesChanged)
        {
            _imageSources.Clear();
            _imageSources.AddRange(imageSources);
            _activeImageIndex = Math.Clamp(_activeImageIndex, 0, Math.Max(0, _imageSources.Count - 1));
            RestartCarousel();
        }
    }

    private List<string> BuildImageSources()
    {
        var imageSources = Restaurant.PhotoList
            .Where(photo => photo.Photo is { Length: > 0 })
            .Select(photo => photo.Photo.ToImage())
            .ToList();

        if (imageSources.Count == 0 && !string.IsNullOrWhiteSpace(Restaurant.ImageUrl))
        {
            imageSources.Add(Restaurant.ImageUrl);
        }

        return imageSources;
    }

    private void ShowPreviousImage()
    {
        if (_imageSources.Count < 2)
        {
            return;
        }

        _activeImageIndex = (_activeImageIndex - 1 + _imageSources.Count) % _imageSources.Count;
    }

    private void ShowNextImage()
    {
        if (_imageSources.Count < 2)
        {
            return;
        }

        _activeImageIndex = (_activeImageIndex + 1) % _imageSources.Count;
    }

    private void SelectImage(int imageIndex)
    {
        if (imageIndex >= 0 && imageIndex < _imageSources.Count)
        {
            _activeImageIndex = imageIndex;
        }
    }

    private void SwitchTab(string tab) => _tab = tab;

    private void ToggleReviewForm() => _showReviewForm = !_showReviewForm;

    private void SubmitReview()
    {
        if (string.IsNullOrWhiteSpace(_newReviewName) || string.IsNullOrWhiteSpace(_newReviewComment))
        {
            return;
        }

        _userReviews.Insert(0, new StaticReview(_newReviewName.Trim(), "Just now", _newReviewRating, _newReviewComment.Trim()));
        _newReviewName = "";
        _newReviewComment = "";
        _newReviewRating = 5;
        _showReviewForm = false;
    }

    private void RestartCarousel()
    {
        StopCarousel();

        if (_imageSources.Count > 1)
        {
            _carouselCancellation = new CancellationTokenSource();
            _carouselTask = RotateImagesAsync(_carouselCancellation.Token);
        }
    }

    private async Task RotateImagesAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await InvokeAsync(ShowNextImage);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private void StopCarousel()
    {
        _carouselCancellation?.Cancel();
        _carouselCancellation?.Dispose();
        _carouselCancellation = null;
        _carouselTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        var carouselTask = _carouselTask;
        StopCarousel();

        if (carouselTask is not null)
        {
            try
            {
                await carouselTask;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}

internal sealed record StaticReview(string Name , string Date , int Rating , string Comment);

internal sealed record RatingMetric(string Label , string BarClass , int Percent , decimal Score);