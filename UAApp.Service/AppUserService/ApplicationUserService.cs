using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAApp.Application.AppUserService.Dto;
using UAApp.Domain.Common;
using UAApp.Domain.Entities;
using UAApp.Infrastructure.Data;
using UAApp.Persistence.DB;
using UAApp.Persistence.Repositories;
using UAApp.Shared.CurrentUser;


namespace UAApp.Application.AppUserService
{
    public class ApplicationUserService : Repository<ApplicationUser>, IApplicationUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IApplicationLogger _logger;
        public ApplicationUserService(ApplicationDbContext context, IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : base(context)
        {
            this._currentUserService = currentUserService;
            this._unitOfWork = unitOfWork;
        }
        public async Task SaveAppUser(AppUserDto appUserDto)
        {
            try
            {
                if (appUserDto.Id.Equals(0))
                {
                    this.Add(new ApplicationUser
                    {
                        FullName = appUserDto.FullName,
                        Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                        PhoneNumber = appUserDto.PhoneNumber,
                        UserEmail = appUserDto.UserEmail,
                    });
                }
                else
                {
                    var appUser = this.Get(appUserDto.Id);
                    if (appUser != null)
                    {
                        appUser.FullName = appUserDto.FullName;
                        appUser.Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password);
                        appUser.PhoneNumber = appUserDto.PhoneNumber;
                        appUser.UserEmail = appUserDto.UserEmail;
                        this.Update(appUser);
                    }
                }
                await this._unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
