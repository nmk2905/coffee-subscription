using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts.DTOs;
using Contracts.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Interface;

namespace Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinarySettings _cloudinarySetting;
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            if (string.IsNullOrEmpty(cloudinaryConfig.Value.CloudName) ||
                string.IsNullOrEmpty(cloudinaryConfig.Value.ApiKey) ||
                string.IsNullOrEmpty(cloudinaryConfig.Value.ApiSecret))
            {
                throw new ArgumentException("Cloudinary settings must be configured properly.");
            }

            var account = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinarySetting = cloudinaryConfig.Value;
        }
        public async Task<DeletionResult> DeleteFileAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deletionParams);
        }

        public async Task<CloudinaryResponse> UploadImage(string fileName, IFormFile fileImage)
        {
            if (fileImage == null || fileImage.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.");
            }

            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileImage.OpenReadStream()),
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult?.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Upload Error: {uploadResult.Error?.Message}");
                    return null;
                }

                var imageUrl = uploadResult.Uri.AbsoluteUri;
                var imageId = uploadResult.PublicId;

                return new CloudinaryResponse
                {
                    ImageUrl = imageUrl,
                    PublicImageId = imageId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during upload: {ex.Message}");
                return null;
            }
        }
    }
}
