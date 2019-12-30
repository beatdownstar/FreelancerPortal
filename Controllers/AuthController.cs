using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProsjektoppgaveITE1811Gruppe7.Models.Repositories;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _repository;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, IAuthRepository repository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _repository = repository;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var signinKey = Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]);
                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
                var cIdentity = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                   //, new Claim(ClaimTypes.Role, user.Role)        
                    
                });

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = cIdentity,
                    Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signinKey), SecurityAlgorithms.HmacSha256Signature),
                };
                
                var token = _repository.GetSecurityToken(tokenHandler, tokenDescriptor);
                var tokenString = _repository.GetTokenString(tokenHandler, token);

                return Ok(new ObjectResult(tokenString));
            }
            return Unauthorized();
        }
    }
}