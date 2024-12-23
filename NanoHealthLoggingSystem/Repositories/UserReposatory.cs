using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NanoHealthLoggingSystem.Configrations;
using NanoHealthLoggingSystem.Context;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.Entities;
using NanoHealthLoggingSystem.IRepositories;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NanoHealthLoggingSystem.Reposatories
{
    public class UserReposatory : IUserReposatory
    {
        private readonly LoggingContext _context;
        private RoleManager<IdentityRole> _roleManager;
        private readonly JWTConfiguration _jwt;
        private readonly SignInManager<User> _signInManager;

        private UserManager<User>  _userManager;
        public UserReposatory(LoggingContext context, RoleManager<IdentityRole> roleManager , IOptions<JWTConfiguration> jwtOptions,
            SignInManager<User> signInManager , UserManager<User> userManager)
        {
            _context = context; 
            _roleManager = roleManager;
            _jwt = jwtOptions.Value;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public ICollection<RoleDto> GetRoles()
        {
            var Roles = _roleManager.Roles.ToList();
            var roleDto = new List<RoleDto>();
            foreach (var role in Roles)
            {
                roleDto.Add(new RoleDto { RoleId =  role.Id , RoleName = role.Name});
            }
            return roleDto; 
        }

        public async Task<UserDto> LogIn(LogInDto logInDto)
        {
            var user = await _userManager.FindByEmailAsync(logInDto.UserEmail);
            if (user is null || !await _userManager.CheckPasswordAsync(user, logInDto.Password))
                throw new Exception("NotFound");

            return new UserDto { Id = user.Id, UserName = user?.UserName??"", Token = await CreateJwtToken(user) };
        }

        public async Task<UserDto> Register(RegisterDto user)
        {
            if (await _userManager.FindByEmailAsync(user.UserEmail) is not null)
                throw new Exception("Exist User Email "+user.UserEmail);
            if (await _userManager.FindByNameAsync(user.UserName) is not null)
                throw new Exception("Exist User Name " + user.UserName);
            var User = new User()
            {
                Email = user.UserEmail,
                UserName = user.UserName,
                
            };

            var result = await _userManager.CreateAsync(User, user.Password);

            if (!result.Succeeded)
                throw new Exception("Create user failed");
            var roleRes = await _userManager.AddToRoleAsync(User, user.RoleId);
            if (!result.Succeeded)
                throw new Exception("adding role failed");

            return new UserDto { Id = User.Id, UserName = User.UserName, Token = await CreateJwtToken(User) };
        }
        private async Task<string> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer, audience: _jwt.Audience,
                claims: claims, expires: DateTime.Now.AddDays(_jwt.AvailavleDays),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
