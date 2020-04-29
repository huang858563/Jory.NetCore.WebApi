using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Jory.NetCore.WebApi.Options
{
    public class JwtBearerOption : IOptions<JwtBearerOption>
    {
        public JwtBearerOption Value => this;
        public bool Enabled { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessExpiration { get; set; }
        public int RefreshExpiration { get; set; }
    }
}
