using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UAApp.Domain.Common;

namespace UAApp.Shared.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IList<Claim> cliam = identity.Claims.ToList();
                if (cliam.Count > 0)
                {
                    Name = cliam[0].Value;
                    Email = cliam[1].Value;
                    UserId = cliam[2].Value;
                    LoginId = cliam[1].Value;
                    AccessLevel = cliam[3].Value;
                    o365AccessToken = cliam[5].Value;
                }
            }
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string LoginId { get; set; }
        public string AccessLevel { get; set; }
        public string o365AccessToken { get; set; }
    }
}
