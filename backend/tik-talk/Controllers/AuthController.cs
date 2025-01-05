using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tik_talk.Dtos;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly UserManager<Auth> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<Auth> _signInManager;
    public AuthController(UserManager<Auth> userManager, ITokenService tokenService, SignInManager<Auth> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
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
                    return Ok(
                        new NewUserDto{
                            UserName = user.UserName,
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user)
                        }
                    );
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginDto loginDto){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());
        if(user ==null) return Unauthorized("Invalid username");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if(!result.Succeeded){
            return Unauthorized("Username not found or password is incorrect");
        }
        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.refreshToken = refreshToken;
        user.refreshTokenExpiry = DateTime.Now.AddHours(12);
        await _userManager.UpdateAsync(user);
        return Ok(
            new LoginResponse{
                accessToken = accessToken,
                refreshToken = refreshToken,
                isLoggedIn = true
            });

    }
  [HttpPost("refresh")]
public async Task<IActionResult> RefreshToken([FromBody]TokenDto tokens)
{
    try
    {
        var username = _tokenService.ExtractUsernameFromToken(tokens.accessToken);
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("Invalid token: username not found");
        }

        var identityUser = await _userManager.FindByNameAsync(username);
        if (identityUser == null || identityUser.refreshToken != tokens.refreshToken || identityUser.refreshTokenExpiry < DateTime.Now)
        {
            return Unauthorized("Invalid refresh token or expired token");
        }

        var response = new LoginResponse
        {
            isLoggedIn = true,
            accessToken = _tokenService.CreateToken(identityUser),
            refreshToken = _tokenService.GenerateRefreshToken()
        };

        identityUser.refreshToken = response.refreshToken;
        identityUser.refreshTokenExpiry = DateTime.Now.AddHours(12);
        await _userManager.UpdateAsync(identityUser);

        return Ok(response);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

}
