using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Hotel.Domain.Abstractions.Services
{
    public interface IImageService
    {
        Task<FileStream?> GetImageStream(Guid roomId);
        Task<string?> GetContentType(Guid roomId);
    }
}
