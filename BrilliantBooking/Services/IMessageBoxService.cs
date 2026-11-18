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

    Task ShowAsync(string message, string title, MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary);
    void ShowMessage(string message , string title);
    void ShowException(Exception ex , string title = "Error");
    void ShowWarning(string message , string title);

    void Close();
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
