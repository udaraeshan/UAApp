using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAApp.Shared.CurrentUser
{
    public interface ICurrentUserService
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}
