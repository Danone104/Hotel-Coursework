using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services
{
    public interface ISystemInitializer
    {
        Task<bool> IsSystemNeedInitializeAsync();
        Task InitializeSystemAsync();
    }
}
