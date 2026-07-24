using System.IdentityModel.Tokens.Jwt;
using TaskManagement.Dtos.Auth;

namespace TaskManagement.ExtensionMethods.Mapping;

public static class JwtSecurityTokenMapping
{
    public static JwtDto ToDto(this JwtSecurityToken token) =>
    new()
    {
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        TokenExpiration = token.ValidTo
    };
}