using BrilliantBooking.Components.Pages;
using BrilliantBooking.Services;
using Data.VO.Restaurant;

namespace BrilliantBooking.Extensions;

public static class ComponentExtensions
{
    public static async Task ShowProcessing(this Task task , PageBase pageBase , string? mainText = null , string? subText = null)
        => await ShowProcessing(task , pageBase.LoadingService , mainText , subText);
    public static async Task ShowProcessing(this Task task , LoadingService loadingService , string? mainText = null , string? subText = null)
    {
        loadingService.Show(mainText , subText);
        try
        {
            await task;
        }
        finally
        {
            loadingService.Hide();
        }
    }

    public static async Task<T> ShowProcessing<T>(this Task<T> task , PageBase pageBase , string? mainText = null , string? subText = null)
        => await ShowProcessing<T>(task , pageBase.LoadingService , mainText , subText);
    public static async Task<T> ShowProcessing<T>(this Task<T> task , LoadingService loadingService , string? mainText = null , string? subText = null)
    {
        loadingService.Show(mainText , subText);
        try
        {
            return await task;
        }
        finally
        {
            loadingService.Hide();
        }
    }

    public static string ToImage(this byte[] bytes , string imageType = "jpeg")
        => $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
}
