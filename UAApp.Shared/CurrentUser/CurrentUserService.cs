using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UAApp.Shared.CurrentUser
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

                }
            }
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}
