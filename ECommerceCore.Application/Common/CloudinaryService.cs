using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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

        //  upload multiple images
        public async Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new BadRequestException("At least one image is required");

            if (files.Count > 5)
                throw new BadRequestException("Maximum 5 images allowed per product");

            var urls = new List<string>();
            foreach (var file in files)
            {
                var url = await UploadImageAsync(file);
                urls.Add(url);
            }
            return urls;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return false;
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        //  delete multiple images
        public async Task DeleteMultipleImagesAsync(List<string> imageUrls)
        {
            foreach (var url in imageUrls)
                await DeleteImageAsync(url);
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var folderAndFile = string.Join("/", segments.Skip(segments.Length - 2));
            return Path.ChangeExtension(folderAndFile, null);
        }
    }
}