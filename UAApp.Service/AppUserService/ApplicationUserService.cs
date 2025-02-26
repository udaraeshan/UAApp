using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UAApp.Application.AppUserService.Dto;
using UAApp.Domain.Entities;
using UAApp.Infrastructure.Data;
using UAApp.Persistence.DB;
using UAApp.Persistence.Repositories;
using UAApp.Shared.AppSettings;
using UAApp.Shared.Constants;
using UAApp.Shared.CurrentUser;
using UAApp.Shared.Exceptions;
using UAApp.Shared.Log;


namespace UAApp.Application.AppUserService
{
    public class ApplicationUserService : Repository<ApplicationUser>, IApplicationUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ApplicationSetting> _applicationSettings;
        private readonly IApplicationLogger _logger;
        public ApplicationUserService(ApplicationDbContext context, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IOptions<ApplicationSetting> applicationSettings, IApplicationLogger logger) : base(context)
        {
            this._currentUserService = currentUserService;
            this._unitOfWork = unitOfWork;
            this._applicationSettings = applicationSettings;
            this._logger = logger;
        }
        public async Task SaveAppUser(AppUserDto appUserDto)
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
        public async Task<AuthResponseDto> Authenticate(AuthRequestDto authRequest)
        {
            bool isAuthenticated = false;
            switch (authRequest.AuthType)
            {
                case "INAPP":
                    var user = this.GetSingleOrDefault(x => x.UserEmail.Equals(authRequest.Email) && x.IsActive);
                    if (user != null)
                    {
                        isAuthenticated = BCrypt.Net.BCrypt.Verify(authRequest.UserPassword, user.Password);
                        authRequest.Name = user.FullName;
                        authRequest.UserId = user.Id.ToString();
                    }
                    break;
                case "MICROSOFT":
                    isAuthenticated = true;
                    break;
                default:
                    break;
            }

            if (isAuthenticated)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(this._applicationSettings.Value.AuthenticationConfig.Key);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name,authRequest.Name),
                    new Claim("Email", authRequest.Email),
                    new Claim("UserId", authRequest.UserId),
                    new Claim (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(this._applicationSettings.Value.AuthenticationConfig.ExpiresIn),
                    SigningCredentials = new
                    SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new AuthResponseDto { Email = authRequest.Email, JwtToken = tokenHandler.WriteToken(token), Name = authRequest.Name, RefreshToken = string.Empty, UserID = authRequest.UserId.ToString(), Success = true };
            }
            else
            {
                _logger.Log(LogLevel.Error, new LogFormat
                {
                    Message = ExceptionStrings.AuthFailedMessage,
                    Description = ExceptionStrings.AuthFailedDescription,
                    UserID = authRequest.Email
                });
                throw new NotFoundException(ExceptionStrings.AuthFailedDescription);
            }
        }
    }
}
