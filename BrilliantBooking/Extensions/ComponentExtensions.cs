using BrilliantBooking.Components.Pages;
using BrilliantBooking.Services;
using Data.DTO;
using Data.VO.Restaurant;
using Microsoft.AspNetCore.Components.Forms;

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

    public static async Task<List<FileDTO>> ToFileDTOs(this InputFileChangeEventArgs e , int maxFiles , long maxFileSize = 10 * 1024 * 1024)
    {
        List<FileDTO> files = new();

        foreach (var file in e.GetMultipleFiles(maxFiles))
        {
            using var stream = file.OpenReadStream(maxAllowedSize: maxFileSize);
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            files.Add(new FileDTO
            {
                Name = file.Name ,
                File = memory.ToArray()
            });
        }

        return files;
    }
}
