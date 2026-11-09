namespace BrilliantBooking.Services;

public class LoadingService
{
    public bool IsLoading { get; private set; }
    public string? MainText { get; private set; } = "Processing";
    public string? SubText { get; private set; } = "Please wait...";

    public event Action? Changed;

    public void Show(string? mainText = null, string? subText = null)
    {
        MainText = mainText ?? MainText;
        SubText = subText ?? SubText;
        IsLoading = true;
        Notify();
    }

    public void Hide()
    {
        IsLoading = false;
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
