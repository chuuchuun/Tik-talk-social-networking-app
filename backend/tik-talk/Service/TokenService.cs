using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using tik_talk.Dtos;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
    }
  public string CreateToken(Auth auth)
  {
    var claims = new List<Claim>{
        new Claim(JwtRegisteredClaimNames.Email, auth.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, auth.UserName)
    };

    var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

    var tokenDescriptor = new SecurityTokenDescriptor{
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddMinutes(30),
        SigningCredentials = creds,
        Issuer = _configuration["JWT:Issuer"],
        Audience = _configuration["JWT:Audience"]
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }

  public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using(var numberGenerator = RandomNumberGenerator.Create()){
            numberGenerator.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }
   public string ExtractUsernameFromToken(string accessToken)
{
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;
    var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
    return username;
}

}
