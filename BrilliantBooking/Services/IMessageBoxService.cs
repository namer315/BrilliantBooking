using BrilliantBooking.Components.Controls;

namespace BrilliantBooking.Services;

public interface IMessageBoxService
{
    event Action? Changed;

    #region MessageBox
    bool IsVisible { get; }

    string Title { get; }

    string Message { get; }

    MessageBox.MessageBoxType Type { get; }

    MessageBox.MessageBoxButtonMode ButtonMode { get; }

    Task<MessageBox.MessageBoxResult> ShowAsync(string message, string title, MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary, MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK);
    void ShowMessage(string message , string title);
    void ShowException(Exception ex , string title = "Error" , MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK);
    Task<MessageBox.MessageBoxResult> ShowExceptionAsync(Exception ex , string title = "Error" , MessageBox.MessageBoxButtonMode buttonMode = MessageBox.MessageBoxButtonMode.OK);
    void ShowWarning(string message , string title);

    void Close(MessageBox.MessageBoxResult result = MessageBox.MessageBoxResult.Cancel);
    #endregion

    #region Toast

    bool IsToastVisible { get; }

    string ToastTitle { get; }

    string ToastMessage { get; }

    MessageBox.MessageBoxType ToastType { get; }

    Task ShowToastAsync(string message, string title, MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary);
    void CloseToast();
    Task ShowToastSuccess(string message , string title);

    #endregion
}
