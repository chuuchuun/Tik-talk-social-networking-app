using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace tik_talk.Models;

public class Auth: IdentityUser
{
    public string? refreshToken { get; set; }
    public DateTime? refreshTokenExpiry { get; set; }
}
