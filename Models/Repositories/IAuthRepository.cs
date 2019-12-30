using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ProsjektoppgaveITE1811Gruppe7.Models.Repositories
{
    public interface IAuthRepository
    {
        SecurityToken GetSecurityToken(JwtSecurityTokenHandler handler, SecurityTokenDescriptor std);

        string GetTokenString(JwtSecurityTokenHandler handler, SecurityToken token);

    }
}
