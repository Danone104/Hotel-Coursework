using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Hotel.Application.Services
{
    public class ImageService : IImageService, IAdminImageService
    {
        private readonly IConfiguration _configuration;
        
        private readonly string _storagePath;
      
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const int PRIMARY_IMAGE_INDEX = 0;
        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var relativePath = configuration.GetSection("ImageSettings")["StoragePath"] ?? throw new ArgumentException("Путь не найден");
            
            _storagePath = Path.Combine(AppContext.BaseDirectory, relativePath);

            Directory.CreateDirectory(_storagePath);
        }
        public async Task UploadImageAsync(Guid roomId, IFormFile image)
        {

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension)) throw new ArgumentException("Загрузите изображение!");
            if (await ImageExistsAsync(roomId)) throw new ArgumentException("Изображение для этой комнаты уже сущеcтвует");
            var fileName = $"room-{roomId}{extension}";
            var filePath = Path.Combine(_storagePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
        }

        private async Task<bool> ImageExistsAsync(Guid roomId)
        {
            var files = Directory.GetFiles(_storagePath, $"room-{roomId}.*");
            return files.Length > 0;
        }
        public async Task UpdateImageAsync(Guid roomId, IFormFile image)
        {

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var fileName = $"room-{roomId}{extension}";
            var filePath = Path.Combine(_storagePath, fileName);
            if (!File.Exists(filePath)) throw new ArgumentException("Изображение не существует");
            File.Delete(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
        }
        public async Task<FileStream?> GetImageStream(Guid roomId)
        {
            var files = Directory.GetFiles(_storagePath, $"room-{roomId}.*");

            if (files.Length == 0) throw new ArgumentException("Изображение не найдено");

            return new FileStream(files[PRIMARY_IMAGE_INDEX], FileMode.Open, FileAccess.Read);
        }
        public async Task<string> GetContentType(Guid roomId)
        {

            var files = Directory.GetFiles(_storagePath, $"room-{roomId}.*");
            if (files.Length == 0) throw new ArgumentException("Не найден файл");

            var extension = Path.GetExtension(files[0]).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp"
            };
        }
        public async Task DeleteImageAsync(Guid roomId)
        {
            
            var files = Directory.GetFiles(_storagePath, $"room-{roomId}.*");  
            var filePath = Path.Combine(_storagePath, files[PRIMARY_IMAGE_INDEX]);
            if (!File.Exists(filePath)) throw new ArgumentException("Изображение не существует");
            File.Delete(filePath);
        }

    }
}
