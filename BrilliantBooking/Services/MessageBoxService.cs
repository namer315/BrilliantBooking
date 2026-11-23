using BrilliantBooking.Components.Controls;
using Data.Exceptions;

namespace BrilliantBooking.Services;

public class MessageBoxService : IMessageBoxService
{
    #region MessageBox
    private TaskCompletionSource<MessageBox.MessageBoxResult>? _completion;

    public event Action? Changed;

    public bool IsVisible { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public MessageBox.MessageBoxType Type { get; private set; } = MessageBox.MessageBoxType.Primary;

    public MessageBox.MessageBoxButtonMode ButtonMode { get; private set; } = MessageBox.MessageBoxButtonMode.OK;

    public async Task<MessageBox.MessageBoxResult> ShowAsync(string message , string title , MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary, MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK)
    {
        _completion?.TrySetResult(MessageBox.MessageBoxResult.Cancel);

        Title = title;
        Message = message;
        Type = type;
        ButtonMode = buttonMode;
        IsVisible = true;
        _completion = new TaskCompletionSource<MessageBox.MessageBoxResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        Changed?.Invoke();

        if (buttonMode == MessageBox.MessageBoxButtonMode.OK)
        {
            return MessageBox.MessageBoxResult.OK;
        }

        return await _completion.Task;
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
    public async Task<MessageBox.MessageBoxResult> ShowMessageBoxDanger(string message , string title = "Error" , MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK)
    {
        return await ShowAsync(message , title , MessageBox.MessageBoxType.Danger, buttonMode);
    }
    //public async void ShowException(Exception ex , string title = "Error")
    //{
    //    _ = ShowAsync(ex.Message , title , MessageBox.MessageBoxType.Danger);
    //}

    public void Close(MessageBox.MessageBoxResult result = MessageBox.MessageBoxResult.Cancel)
    {
        IsVisible = false;
        _completion?.TrySetResult(result);
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

    public async Task ShowToastSuccess(string message , string title = "Confirmation")
    {
        await ShowToastAsync(message , title , MessageBox.MessageBoxType.Success);
    }
    public async Task ShowToastDanger(string message , string title = "Error")
    {
        await ShowToastAsync(message , title , MessageBox.MessageBoxType.Danger);
    }

    #endregion

    #region General
    public async void ShowMessage(string message , string title = "Confirmation")
    {
        await ShowToastSuccess(message , title);
    }
    public async Task<MessageBox.MessageBoxResult> ShowExceptionAsync(Exception ex , string title = "Error" , MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK)
    {
        switch (ex)
        {
            case AppException appException when buttonMode == MessageBox.MessageBoxButtonMode.OK:
                await ShowToastDanger(ex.Message , title);
                return MessageBox.MessageBoxResult.OK;
                break;
            default:
                return await ShowMessageBoxDanger(ex.Message , title, buttonMode);
                break;
        }

    }
    public void ShowException(Exception ex , string title = "Error" , MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK)
    {
        _ = ShowExceptionAsync(ex , title , buttonMode);
    }
    #endregion
}
