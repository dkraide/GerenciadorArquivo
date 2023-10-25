using Communication.Contexts;
using Communication.DTOs;
using Communication.Models;
using Communication.Services;
using Communication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly SUser _userService;
        private readonly UserManager<MUser> _userManager;
        private readonly SignInManager<MUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public UserController(
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,  
            Context context,
            UserManager<MUser> userManager, 
            SignInManager<MUser> signInManager)
        {
            _userService = new SUser(context, userManager);
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login([FromBody] UserLogin Data)
        {

            var user = await _userManager.FindByNameAsync(Data.UserName);
            PasswordHasher<MUser> passwordHasher = new PasswordHasher<MUser>();
            if (user != null)
            {
                if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Data.Password) != PasswordVerificationResult.Failed)
                {
                    return await BuildToken(user);
                }
            }
            ModelState.AddModelError(string.Empty, "login inválido.");
            return BadRequest(ModelState);
        }
        private async Task<object> BuildToken(MUser userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim("Name", userInfo.Name),
                new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id),
                new Claim("id", userInfo.Id)
            };
            var userRoles = await _userManager.GetRolesAsync(userInfo);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(48);
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);
            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                User = new UserDTO
                {
                    Email = userInfo.Email,
                    Files  = null,
                    Id = userInfo.Id,
                    Name = userInfo.Name,
                    UserName = userInfo.UserName,
                },
            };
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(MUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid fields");

                var res = await _userService.Create(user);
                UserDTO userDto = new UserDTO
                {
                    Email = user.Email,
                    Files = null,
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid Fields");

                var user = _userService.GetUser(userDTO.Id);
                user.UserName = userDTO.UserName;
                user.Name = userDTO.Name;
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    return Ok(userDTO);
                }
                else
                    throw new Exception(res.Errors.First().Description);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string Id, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    throw new Exception("Usuário não encontrado");
                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var res = await _userManager.ResetPasswordAsync(user, token, password);

                if (!res.Succeeded)
                    throw new Exception("Erro ao atualizar senha");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public class UserLogin
    {
        public string UserName { get; set;}
        public string Password { get; set; }
    }
}
