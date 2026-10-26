using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Security
{
    public class JwtOptions 
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresHours { get; set; }
    }
}
