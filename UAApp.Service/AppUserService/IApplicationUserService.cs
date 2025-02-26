using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAApp.Application.AppUserService.Dto;

namespace UAApp.Application.AppUserService
{
    public interface IApplicationUserService
    {
        Task SaveAppUser(AppUserDto appUserDto);
    }
}
