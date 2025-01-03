using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using tik_talk.Data;
using tik_talk.Interfaces;
using tik_talk.Models;
using System.Security.Claims;
using tik_talk.Dtos;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace tik_talk.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IAccountRepository _accountRepo;
    private readonly ITokenService _tokenservice;
    public AccountController(ApplicationDBContext context, IAccountRepository accountRepository, ITokenService tokenService){
        _context = context;
        _accountRepo = accountRepository;
        _tokenservice = tokenService;

    }

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var accounts = await _accountRepo.GetAllAsync();
        //var stockDTO = accounts.Select(s => s.ToStockDto()).ToList();
        return Ok(accounts);
    }


    [HttpPost]
        public async Task<IActionResult> Create([FromBody] Account account){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            //var stockModel = stockDto.ToStockFromCreateRequestDto();
            await _accountRepo.CreateAsync(account);
            return Ok(account);
        }
    [HttpPatch]
    [Route("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Account accountDTO){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var account = await _accountRepo.UpdateAsync(id, accountDTO);
        if(account == null){
            return NotFound();
        }
        return Ok(account);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var account = await _accountRepo.DeleteAsync(id);
        return NoContent();
    } 
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyAccount()
    {
        // Extract the token from the Authorization header
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Token is missing.");
        }

        // Decode the token and extract claims
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        // Extract the 'given_name' claim
        var givenName = jsonToken?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

        if (string.IsNullOrEmpty(givenName))
        {
            return Unauthorized("Given name not found in the token.");
        }

        // Now you can use the givenName (for example, to fetch the account)
        var account = await _accountRepo.GetByUsernameAsync(givenName);

        if (account == null)
        {
            return NotFound("Account not found.");
        }

        var accountDto = new AccountDto
        {
            Id = account.Id,
            Username = account.username,
            AvatarUrl = account.avatarUrl,
            SubscribersAmount = account.subscribersAmmount,
            FirstName = account.firstName,
            LastName = account.lastName,
            IsActive = account.isActive,
            Stack = account.stack,
            City = account.city,
            Description = account.description
        };

        return Ok(accountDto);
    }
}

