using CloudinaryDotNet.Actions;
using Contracts.DTOs;
using Microsoft.AspNetCore.Http;

namespace Services.Interface
{
    public interface ICloudinaryService
    {
        Task<CloudinaryResponse> UploadImage(string fileName, IFormFile fileImage);
        Task<DeletionResult> DeleteFileAsync(string publicId);
    }
}
