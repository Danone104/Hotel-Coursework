using Hotel.API.Contracts;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;

namespace Hotel.API.Controllers
{
    [Route("api/authentication/")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authenticationService;
        private IAuthenticationClientService _clientAuthenicationService;
        public AuthenticationController(IAuthenticationService authenticationService, IAuthenticationClientService clientAuthenicationService)
        {
            _authenticationService = authenticationService;
            _clientAuthenicationService = clientAuthenicationService;
            }
            [HttpPost("login")]
            public async Task<ActionResult> Login([FromBody] LoginRequestDto user)
            {
                try
                {
                    string token = await _authenticationService.Login(user.UserName, user.Password);
                    Response.Cookies.Append("access_token", token);   
                
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { ex.Message });
                }
            }
        [HttpGet("user-profile")]
        public async Task<ActionResult<UserProfileResponse>> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole =User.FindFirstValue(ClaimTypes.Role);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (Guid.TryParse(userId, out var result))
                return Ok(new UserProfileResponse(result,userRole,userName));
                
                

            return BadRequest(new { message="Пользователь не зарегистрирован"});
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto user)
        {
            try
            {
                await _clientAuthenicationService.Register(user.UserName,user.Password,
                    user.Phone,user.FirstName,user.LastName,user.Patronymic);
                return Ok();
            }
            catch (Exception ex)
            {       
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok();    
        }
        [HttpGet("name-availability")]
        public async Task<ActionResult<bool>> IsNameAvailable([FromQuery] string userName)
        {
            return await _clientAuthenicationService.IsNameAvailable(userName);
        }
    }
}
    