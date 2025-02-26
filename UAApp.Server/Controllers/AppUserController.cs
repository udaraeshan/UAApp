using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UAApp.Application.AppUserService.Dto;
using UAApp.Application.AppUserService;
using UAApp.Domain.Common;
using UAApp.Infrastructure.Data;

namespace UAApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserService;
        public AppUserController(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveAppUser([FromBody] AppUserDto appUserDto)
        {
            if (appUserDto == null)
            {
                return BadRequest("Invalid user data.");
            }
            await _applicationUserService.SaveAppUser(appUserDto);
            return Ok("User saved successfully.");
        }
    }

}