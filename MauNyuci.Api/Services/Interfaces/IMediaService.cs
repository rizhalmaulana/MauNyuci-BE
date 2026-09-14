using Microsoft.AspNetCore.Http;

namespace MauNyuci.Api.Services.Interfaces
{
    public interface IMediaService
    {
        // Fungsi ini akan mengembalikan URL gambar publik yang siap disimpan ke database
        Task<string> UploadImageAsync(IFormFile file, string folderName);
    }
}