using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAApp.Application.AppUserService;
using UAApp.Application.AppUserService.Dto;
using UAApp.Shared.Constants;
using UAApp.Shared.Exceptions;

namespace UAApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserService;
        public AuthController(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequestDto authRequestDto)
        {
            try
            {
                if (authRequestDto == null)
                {
                    return BadRequest(ApiStrings.BadRequest);
                }
                var response = await _applicationUserService.Authenticate(authRequestDto);
                return Ok(new { message = ApiStrings.SuccessRequest, status = 200, data = response });
            }
            catch (NotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }

        }
    }
}
