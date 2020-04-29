using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jory.NetCore.WebApi.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }

        public DateTime AccessExpiration { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshExpiration { get; set; }

        public bool Success { get; set; }
    }
}
