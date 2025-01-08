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
        public async Task<IActionResult> Create([FromBody] AccountDto account){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            //var stockModel = stockDto.ToStockFromCreateRequestDto();
            await _accountRepo.CreateAsync(account.ToAccount());
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
    [HttpDelete("me")]
        [EnableCors("AllowFrontend")] 
    public async Task<IActionResult> DeleteMyAccount()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        try
        {
            var account = await _accountRepo.GetAccountFromTokenAsync(token);
            await _accountRepo.DeleteAsync(account.Id);

            return NoContent();
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
public async Task<IActionResult> MySubscribers(
    [FromQuery] string? stack = null,
    [FromQuery] string? firstLastName = null,
    [FromQuery] string? city = null,
    [FromQuery] string? orderBy = "id",
    [FromQuery] int page = 1,
    [FromQuery] int size = 50)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);

    if (myAccount == null)
    {
        return NotFound("Account not found.");
    }

    var subscribersUsernames = myAccount.subscribers;

   var subscriberAccounts = new List<Account>();

foreach (var username in subscribersUsernames)
{
    var account = await _accountRepo.GetByUsernameAsync(username);
    if (account != null)
    {
        subscriberAccounts.Add(account);
    }
}

    var validSubscriberAccounts = subscriberAccounts.Where(account => account != null).ToList();
    if (!string.IsNullOrEmpty(stack))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => account.stack.Contains(stack))
            .ToList();
    }

    if (!string.IsNullOrEmpty(firstLastName))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => (account.firstName + " " + account.lastName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase) ||
                              (account.lastName + " " + account.firstName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    if (!string.IsNullOrEmpty(city))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => account.city == city)
            .ToList();
    }

    var subscribers = validSubscriberAccounts.Select(account => new AccountDto
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
    }).ToList();

    subscribers = orderBy.ToLower() switch
    {
        "username" => subscribers.OrderBy(s => s.Username).ToList(),
        "subscribersamount" => subscribers.OrderByDescending(s => s.SubscribersAmount).ToList(),
        "city" => subscribers.OrderBy(s => s.City).ToList(),
        _ => subscribers.OrderBy(s => s.Id).ToList(),
    };

    var total = subscribers.Count;
    var totalPages = (int)Math.Ceiling((double)total / size);
    var paginatedSubscribers = subscribers
        .Skip((page - 1) * size)
        .Take(size)
        .ToList();

    var response = new
    {
        Items = paginatedSubscribers,
        Total = total,
        Page = page,
        Size = size,
        Pages = totalPages
    };

    return Ok(response);
}

[Authorize]
[HttpGet("subscriptions")]
[EnableCors("AllowFrontend")]
public async Task<IActionResult> MySubscriptions( [FromQuery] string? stack = null,
    [FromQuery] string? firstLastName = null,
    [FromQuery] string? city = null,
    [FromQuery] string? orderBy = "id",
    [FromQuery] int page = 1,
    [FromQuery] int size = 50)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    var account = await _accountRepo.GetAccountFromTokenAsync(token);

    if (account == null)
    {
        return NotFound("Account not found.");
    }

    var subscribersUsernames = account.subscriptions;

    var subscriberAccounts = await Task.WhenAll(
        subscribersUsernames.Select(async username => await _accountRepo.GetByUsernameAsync(username))
    );
    var validSubscriberAccounts = subscriberAccounts.Where(account => account != null).ToList();
    if (!string.IsNullOrEmpty(stack))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => account.stack.Contains(stack))
            .ToList();
    }

    if (!string.IsNullOrEmpty(firstLastName))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => (account.firstName + " " + account.lastName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase) ||
                              (account.lastName + " " + account.firstName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    if (!string.IsNullOrEmpty(city))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(account => account.city == city)
            .ToList();
    }

    var subscribers = validSubscriberAccounts.Select(account => new AccountDto
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
    }).ToList();

    subscribers = orderBy.ToLower() switch
    {
        "username" => subscribers.OrderBy(s => s.Username).ToList(),
        "subscribersamount" => subscribers.OrderByDescending(s => s.SubscribersAmount).ToList(),
        "city" => subscribers.OrderBy(s => s.City).ToList(),
        _ => subscribers.OrderBy(s => s.Id).ToList(),
    };

    var total = subscribers.Count;
    var totalPages = (int)Math.Ceiling((double)total / size);
    var paginatedSubscribers = subscribers
        .Skip((page - 1) * size)
        .Take(size)
        .ToList();

    var response = new
    {
        Items = paginatedSubscribers,
        Total = total,
        Page = page,
        Size = size,
        Pages = totalPages
    };

    return Ok(response);
}
}