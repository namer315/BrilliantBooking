using BrilliantBooking.Components.Controls;

namespace BrilliantBooking.Services;

public class MessageBoxService : IMessageBoxService
{
    private TaskCompletionSource? _completion;

    public event Action? Changed;

    public bool IsVisible { get; private set; }

    public string Title { get; private set; } = string.Empty;
        
    public string Message { get; private set; } = string.Empty;

    public MessageBox.MessageBoxType Type { get; private set; } = MessageBox.MessageBoxType.Primary;

    public async Task ShowAsync(string message, string title , MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary)
    {
        _completion?.TrySetResult();

        Title = title;
        Message = message;
        Type = type;
        IsVisible = true;
        _completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Changed?.Invoke();
        await _completion.Task;
    }


    public void ShowMessage(string message , string title = "Confirmation")
    {
        _= ShowAsync(message , title , MessageBox.MessageBoxType.Success);
    }
    public async void ShowInfo(string message , string title = "Info")
    {
        _ = ShowAsync(message , title , MessageBox.MessageBoxType.Info);
    }
    public void ShowWarning(string message , string title = "Warning")
    {
        _ = ShowAsync(message , title , MessageBox.MessageBoxType.Warning);
    }
    public async void ShowError(string message , string title = "Error")
    {
        _ = ShowAsync(message , title , MessageBox.MessageBoxType.Danger);
    }
    public async void ShowException(Exception ex , string title = "Error")
    {
        _ = ShowAsync(ex.Message , title , MessageBox.MessageBoxType.Danger);
    }

    public void Close()
    {
        IsVisible = false;
        _completion?.TrySetResult();
        _completion = null;

        Changed?.Invoke();
    }
}
