using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAApp.Shared.AppSettings
{
    public class ApplicationSetting
    {
        public required AuthenticationConfig AuthenticationConfig { get; set; }
    }
    public class AuthenticationConfig
    {
        public required string Key { get; set; }
        public int ExpiresIn { get; set; }
    }
}
