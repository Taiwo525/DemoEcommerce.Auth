using Auth.Application.Dtos;
using Auth.Application.Interfaces;
using Ecommerce.SharedLibrary.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IUser userRepo) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<Response>> Register(AppUserDto appUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await userRepo.Register(appUserDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Response>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await userRepo.Login(loginDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("id")]
        [Authorize]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid user Id");

            var user = await userRepo.GetUser(id);
            return user is not null ? Ok(user) : NotFound("User is not found");
        }
    }
}
