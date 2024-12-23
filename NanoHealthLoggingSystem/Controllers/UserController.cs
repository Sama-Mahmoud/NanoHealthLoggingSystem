using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.IRepositories;

namespace NanoHealthLoggingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserReposatory _userRepo;

        public UserController(IUserReposatory userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto User)
        {
            try
            {
                var userdto =await _userRepo.Register(User);
                return Ok(userdto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(LogInDto User)
        {
            try
            {
                var userdto = await _userRepo.LogIn(User);
                return Ok(userdto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetRoles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _userRepo.GetRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
