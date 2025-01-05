using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Interfaces;

public interface ITokenService
{
    string CreateToken(Auth auth);
    string GenerateRefreshToken();
    string ExtractUsernameFromToken(string token);
}
