using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
    }
    public async Task<string> UploadImageAsync(string filePath)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            PublicId = Path.GetFileNameWithoutExtension(filePath),
            Overwrite = true
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString();
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        var publicId = GetPublicIdFromUrl(imageUrl);
        if (!string.IsNullOrEmpty(publicId))
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
            {
                throw new Exception("Failed to delete QR Code from Cloudinary");
            }
        }
    }

    // Hàm lấy PublicId từ URL của Cloudinary
    private string GetPublicIdFromUrl(string imageUrl)
    {
        Uri uri = new Uri(imageUrl);
        string path = uri.AbsolutePath;
        string filename = Path.GetFileNameWithoutExtension(path);
        return filename;
    }
}
