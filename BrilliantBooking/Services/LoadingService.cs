using Data.Localization;

namespace BrilliantBooking.Services;

public class LoadingService
{
    public bool IsLoading { get; private set; }
    public string? MainText { get; private set; } = Resources.Processing;
    public string? SubText { get; private set; } = Resources.PleaseWait;

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
