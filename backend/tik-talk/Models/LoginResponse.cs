using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Models;

public class LoginResponse
{
    public bool isLoggedIn { get; set; }
    public string accessToken { get; set; }
    public string refreshToken { get; set; }
}
