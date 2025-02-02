using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class RegisterDto
{
    [Required]
    public string? username { get; set; }
    [Required]
    [EmailAddress]
    public  string? email { get; set; }
    [Required]
    public string? password { get; set; }
}
