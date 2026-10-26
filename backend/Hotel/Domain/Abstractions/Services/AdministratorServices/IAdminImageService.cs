using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.AdministratorServices
{
    public interface IAdminImageService:IImageService
    {
        Task UpdateImageAsync(Guid roomId, IFormFile image);
        Task UploadImageAsync(Guid roomId, IFormFile image);
        Task DeleteImageAsync(Guid roomId);
    }
}
