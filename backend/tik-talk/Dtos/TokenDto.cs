using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class TokenDto
{
    public string accessToken { get; set; }
    public string refreshToken { get; set; }
}
