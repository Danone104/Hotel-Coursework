using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Security
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
