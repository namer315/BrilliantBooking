using BrilliantBooking.Components.Controls;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Controls;

public partial class Toast
{
    private readonly string _titleId = $"toast-title-{Guid.NewGuid():N}";
    private readonly string _messageId = $"toast-message-{Guid.NewGuid():N}";

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public string Message { get; set; } = string.Empty;

    [Parameter] public MessageBox.MessageBoxType Type { get; set; } = MessageBox.MessageBoxType.Primary;

    [Parameter] public bool IsVisible { get; set; }

    [Parameter] public EventCallback OnClose { get; set; }

    private string _iconClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "fa-circle-info",
        MessageBox.MessageBoxType.Success => "fa-circle-check",
        MessageBox.MessageBoxType.Warning => "fa-triangle-exclamation",
        MessageBox.MessageBoxType.Danger => "fa-circle-xmark",
        _ => "fa-star"
    };

    private string _iconBgClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "bg-blue-50 dark:bg-blue-950",
        MessageBox.MessageBoxType.Success => "bg-green-50 dark:bg-green-950",
        MessageBox.MessageBoxType.Warning => "bg-orange-50 dark:bg-orange-950",
        MessageBox.MessageBoxType.Danger => "bg-red-50 dark:bg-red-950",
        _ => "bg-blue-50 dark:bg-blue-950"
    };

    private string _iconTextClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "text-apple-blue",
        MessageBox.MessageBoxType.Success => "text-apple-green",
        MessageBox.MessageBoxType.Warning => "text-apple-orange",
        MessageBox.MessageBoxType.Danger => "text-apple-red",
        _ => "text-apple-blue"
    };

    private string _accentClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "border-l-4 border-l-apple-blue",
        MessageBox.MessageBoxType.Success => "border-l-4 border-l-apple-green",
        MessageBox.MessageBoxType.Warning => "border-l-4 border-l-apple-orange",
        MessageBox.MessageBoxType.Danger => "border-l-4 border-l-apple-red",
        _ => "border-l-4 border-l-apple-dark-gray"
    };

    private string _progressTrackClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "bg-blue-100 dark:bg-blue-950",
        MessageBox.MessageBoxType.Success => "bg-green-100 dark:bg-green-950",
        MessageBox.MessageBoxType.Warning => "bg-orange-100 dark:bg-orange-950",
        MessageBox.MessageBoxType.Danger => "bg-red-100 dark:bg-red-950",
        _ => "bg-stone-200 dark:bg-stone-800"
    };

    private string _progressClass => Type switch
    {
        MessageBox.MessageBoxType.Info => "bg-apple-blue",
        MessageBox.MessageBoxType.Success => "bg-apple-green",
        MessageBox.MessageBoxType.Warning => "bg-apple-orange",
        MessageBox.MessageBoxType.Danger => "bg-apple-red",
        _ => "bg-apple-dark-gray"
    };

    private Task HandleClose() => OnClose.InvokeAsync();
}
