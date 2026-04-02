using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerceCore.Application.Common;
using ECommerceCore.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ECommerceCore.Application.Common
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> settings)
        {
            var account = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("Image file is required");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "ecommerce-products"
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            // extracts "ecommerce-products/filename" from the Cloudinary URL
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var folderAndFile = string.Join("/", segments.Skip(segments.Length - 2));
            return Path.ChangeExtension(folderAndFile, null);
        }
    }
}