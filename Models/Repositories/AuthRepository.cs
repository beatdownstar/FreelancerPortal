using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        public SecurityToken GetSecurityToken(JwtSecurityTokenHandler handler, SecurityTokenDescriptor std)
        {
            
            return handler.CreateToken(std); 

        }

        public string GetTokenString(JwtSecurityTokenHandler handler, SecurityToken token)
        {
            return handler.WriteToken(token);

        }
    }
}
