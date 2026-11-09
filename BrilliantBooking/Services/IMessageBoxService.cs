using BrilliantBooking.Components.Controls;

namespace BrilliantBooking.Services;

public interface IMessageBoxService
{
    event Action? Changed;

    bool IsVisible { get; }

    string Title { get; }

    string Message { get; }

    MessageBox.MessageBoxType Type { get; }

    Task ShowAsync(string message, string title, MessageBox.MessageBoxType type = MessageBox.MessageBoxType.Primary);
    void ShowMessage(string message , string title);
    void ShowException(Exception ex , string title);
    void ShowWarning(string message , string title);

    void Close();
}
