using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly UserManager<Auth> _userManager;
    public AuthController(UserManager<Auth> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto){
        try{
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var user = new Auth{
                UserName = registerDto.username,
                Email = registerDto.email
            };
            var createdUser = await _userManager.CreateAsync(user, registerDto.password);
            if(createdUser.Succeeded){
                var roleResult = await _userManager.AddToRoleAsync(user,"User");
                if(roleResult.Succeeded){
                    return Ok("User created");
                }else{
                    return StatusCode(500, roleResult.Errors);
                }
            }else{
                return StatusCode(500, createdUser.Errors);
            }
        }catch(Exception e){
            return StatusCode(500, e);
        }
    }

}
