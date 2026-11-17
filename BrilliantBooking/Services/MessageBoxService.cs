using BrilliantBooking.Components.Controls;
using Data.Exceptions;

namespace BrilliantBooking.Services;

public class MessageBoxService : IMessageBoxService
{
    #region MessageBox
    private TaskCompletionSource? _completion;

    public event Action? Changed;

    public bool IsVisible { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public MessageBox.MessageBoxType Type { get; private set; } = MessageBox.MessageBoxType.Primary;

    public async Task ShowAsync(string message , string title , MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary)
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


    public async Task ShowMessageBoxSuccess(string message , string title = "Confirmation")
    {
        await ShowAsync(message , title , MessageBox.MessageBoxType.Success);
    }
    public async void ShowInfo(string message , string title = "Info")
    {
        _ = ShowAsync(message , title , MessageBox.MessageBoxType.Info);
    }
    public void ShowWarning(string message , string title = "Warning")
    {
        _ = ShowAsync(message , title , MessageBox.MessageBoxType.Warning);
    }
    public async Task ShowMessageBoxDanger(string message , string title = "Error")
    {
        await ShowAsync(message , title , MessageBox.MessageBoxType.Danger);
    }
    //public async void ShowException(Exception ex , string title = "Error")
    //{
    //    _ = ShowAsync(ex.Message , title , MessageBox.MessageBoxType.Danger);
    //}

    public void Close()
    {
        IsVisible = false;
        _completion?.TrySetResult();
        _completion = null;

        Changed?.Invoke();
    }
    #endregion

    #region Toast

    private TaskCompletionSource? _toastCompletion;
    private CancellationTokenSource? _toastCancellation;
    private long _toastVersion;
    public bool IsToastVisible { get; private set; }

    public string ToastTitle { get; private set; } = string.Empty;

    public string ToastMessage { get; private set; } = string.Empty;

    public MessageBox.MessageBoxType ToastType { get; private set; } = MessageBox.MessageBoxType.Primary;

    public async Task ShowToastAsync(string message , string title , MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary)
    {
        _toastCancellation?.Cancel();
        _toastCancellation?.Dispose();
        _toastCompletion?.TrySetResult();

        ToastTitle = title;
        ToastMessage = message;
        ToastType = type;
        IsToastVisible = true;

        var version = ++_toastVersion;
        var cancellation = new CancellationTokenSource();
        _toastCancellation = cancellation;
        _toastCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Changed?.Invoke();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(4) , cancellation.Token);

            if (version == _toastVersion)
            {
                CloseToast();
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
    }

    public void CloseToast()
    {
        ++_toastVersion;
        IsToastVisible = false;
        _toastCancellation?.Cancel();
        _toastCancellation?.Dispose();
        _toastCancellation = null;
        _toastCompletion?.TrySetResult();
        _toastCompletion = null;

        Changed?.Invoke();
    }

    public void ShowToastSuccess(string message , string title = "Confirmation")
    {
        _ = ShowToastAsync(message , title , MessageBox.MessageBoxType.Success);
    }
    public async Task ShowToastDanger(string message , string title = "Error")
    {
        await ShowToastAsync(message , title , MessageBox.MessageBoxType.Danger);
    }

    #endregion

    #region General
    public async void ShowMessage(string message , string title = "Confirmation")
    {
        await ShowMessageBoxSuccess(message , title);
    }
    public async void ShowException(Exception ex , string title = "Error")
    {
        switch (ex)
        {
            case AppException appException:
                await ShowToastDanger(ex.Message , title);
                break;
            default:
                await ShowMessageBoxDanger(ex.Message , title);
                break;
        }

    }
    #endregion
}
