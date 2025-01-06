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
using Microsoft.AspNetCore.Cors;
using tik_talk.Mappers;

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
        var accountDTO = accounts.Select(s => s.ToAccountDto()).ToList();
        return Ok(accountDTO);
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

      [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GeAccountById([FromRoute] int id){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var account = await _accountRepo.GetByIdAsync(id);
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
        [EnableCors("AllowFrontend")] 

    public async Task<IActionResult> GetMyAccount()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        try
        {
            var account = await _accountRepo.GetAccountFromTokenAsync(token);
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
     
    }
    [Authorize]
[HttpPost("subscribe/{id:int}")]
public async Task<IActionResult> Subscribe([FromRoute] int id)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    var currentAccount = await _accountRepo.GetAccountFromTokenAsync(token);
    
    if (currentAccount == null)
    {
        return NotFound("Current account not found.");
    }

    var targetAccount = await _accountRepo.GetByIdAsync(id);
    if (targetAccount == null)
    {
        return NotFound("Target account not found.");
    }

    var success = await _accountRepo.SubscribeAsync(currentAccount.Id, targetAccount.Id);
    if (!success)
    {
        return BadRequest("Failed to subscribe.");
    }

    return Ok(new { message = "Successfully subscribed." });
}
   [Authorize]
[HttpGet("subscribers")]
[EnableCors("AllowFrontend")]
public async Task<IActionResult> MySubscribers()
{
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

    // Fetch the account
    var account = await _accountRepo.GetByUsernameAsync(givenName);

    if (account == null)
    {
        return NotFound("Account not found.");
    }

    // Fetch the subscribers (AccountDto list)
    var subscribers = new List<AccountDto>();
    foreach (var subscriberUsername in account.subscribers)
    {
        var subscriberAccount = await _accountRepo.GetByUsernameAsync(subscriberUsername);
        if (subscriberAccount != null)
        {
            subscribers.Add(new AccountDto
            {
                Id = subscriberAccount.Id,
                Username = subscriberAccount.username,
                AvatarUrl = subscriberAccount.avatarUrl,
                SubscribersAmount = subscriberAccount.subscribersAmmount,
                FirstName = subscriberAccount.firstName,
                LastName = subscriberAccount.lastName,
                IsActive = subscriberAccount.isActive,
                Stack = subscriberAccount.stack,
                City = subscriberAccount.city,
                Description = subscriberAccount.description
            });
        }
    }

    return Ok(subscribers);
}

[Authorize]
[HttpGet("subscriptions")]
[EnableCors("AllowFrontend")]
public async Task<IActionResult> MySubscriptions()
{
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

    // Fetch the account
    var account = await _accountRepo.GetByUsernameAsync(givenName);

    if (account == null)
    {
        return NotFound("Account not found.");
    }

    // Fetch the subscriptions (AccountDto list)
    var subscriptions = new List<AccountDto>();
    foreach (var subscriptionUsername in account.subscriptions)
    {
        var subscriptionAccount = await _accountRepo.GetByUsernameAsync(subscriptionUsername);
        if (subscriptionAccount != null)
        {
            subscriptions.Add(new AccountDto
            {
                Id = subscriptionAccount.Id,
                Username = subscriptionAccount.username,
                AvatarUrl = subscriptionAccount.avatarUrl,
                SubscribersAmount = subscriptionAccount.subscribersAmmount,
                FirstName = subscriptionAccount.firstName,
                LastName = subscriptionAccount.lastName,
                IsActive = subscriptionAccount.isActive,
                Stack = subscriptionAccount.stack,
                City = subscriptionAccount.city,
                Description = subscriptionAccount.description
            });
        }
    }

    return Ok(subscriptions);
}
}