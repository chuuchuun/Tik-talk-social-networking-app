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
using Microsoft.IdentityModel.Tokens;
using Amazon.S3;
using Amazon.S3.Transfer;
using dotenv.net;



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
    public async Task<IActionResult> GetAll(
        [FromQuery] string? username = null,
        [FromQuery] string? stack = null,
        [FromQuery] string? firstLastName = null,
        [FromQuery] string? city = null,
        [FromQuery] string? orderBy = "id",
        [FromQuery] int page = 1,
        [FromQuery] int size = 50)
    {
        if (page <= 0 || size <= 0) return BadRequest(new { message = "Page and size must be greater than zero." });

        var accounts = await _accountRepo.GetAllAsync();
        if(!string.IsNullOrEmpty(username)){
            var usernameFilter = username.ToLower();
                accounts = accounts
                    .Where(account =>
                        (!string.IsNullOrEmpty(account.username)) &&
                        account.username.ToLower().Contains(usernameFilter)                
                    )
                .ToList();
        }
        if (!string.IsNullOrEmpty(stack))
        {
            accounts = accounts
                .Where(account => account.stack.Contains(stack))
                .ToList();
        }

        if (!string.IsNullOrEmpty(firstLastName))
        {
            var nameFilter = firstLastName.ToLower();
            accounts = accounts
                .Where(account =>
                    !string.IsNullOrEmpty(account.firstName) && !string.IsNullOrEmpty(account.lastName) &&
                    (
                        (account.firstName + " " + account.lastName).ToLower().Contains(nameFilter) ||
                        (account.lastName + " " + account.firstName).ToLower().Contains(nameFilter)
                    ))
                .ToList();
        }

        if (!string.IsNullOrEmpty(city))
        {
            accounts = accounts
                .Where(account => !string.IsNullOrEmpty(account.city) && account.city.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        var accountDTO = accounts
            .Select(account => account.ToAccountDto())
            .ToList();

        accountDTO = orderBy.ToLower() switch
        {
            "username" => accountDTO.OrderBy(dto => dto.Username).ToList(),
            "subscribersamount" => accountDTO.OrderByDescending(dto => dto.SubscribersAmount).ToList(),
            "city" => accountDTO.OrderBy(dto => dto.City).ToList(),
            _ => accountDTO.OrderBy(dto => dto.Id).ToList(),
        };

        var total = accountDTO.Count;
        var totalPages = (int)Math.Ceiling((double)total / size);

        var paginatedAccounts = accountDTO
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var response = new
        {
            Items = paginatedAccounts,
            Total = total,
            Page = page,
            Size = size,
            Pages = totalPages
        };

        return Ok(response);
    }



    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccountDto account){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        await _accountRepo.CreateAsync(account.ToAccount());
        return Ok(account);
    }

    [HttpPatch]
    [Route("me")]
    public async Task<IActionResult> Update([FromBody] UpdateAccount accountDTO)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
            var currentAccount = await _accountRepo.GetAccountFromTokenAsync(token);

            if (currentAccount == null)
            {
                return NotFound("Current account not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _accountRepo.UpdateAsync(currentAccount.Id, accountDTO);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        catch 
        {
            return StatusCode(500, "Internal server error.");
        }
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
[HttpPost("subscribe")]
public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDto sub)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    var currentAccount = await _accountRepo.GetAccountFromTokenAsync(token);
    
    if (currentAccount == null)
    {
        return NotFound("Current account not found.");
    }

    var targetAccount = await _accountRepo.GetByIdAsync(sub.Id);
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
public async Task<IActionResult> MySubscriptions( 
    [FromQuery] string? stack = null,
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

    var subscriptionsUsernames = account.subscriptions;

   var subscriptionAccounts = await _context.Accounts
    .Where(a => subscriptionsUsernames.Contains(a.username))
    .ToListAsync();


    var validSubscriptionAccounts = subscriptionAccounts.Where(account => account != null).ToList();
    if (!string.IsNullOrEmpty(stack))
    {
        validSubscriptionAccounts = validSubscriptionAccounts
            .Where(account => account.stack.Contains(stack))
            .ToList();
    }

    if (!string.IsNullOrEmpty(firstLastName))
    {
        validSubscriptionAccounts = validSubscriptionAccounts
            .Where(account => (account.firstName + " " + account.lastName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase) ||
                              (account.lastName + " " + account.firstName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    if (!string.IsNullOrEmpty(city))
    {
        validSubscriptionAccounts = validSubscriptionAccounts
            .Where(account => account.city == city)
            .ToList();
    }

    var subscriptions = validSubscriptionAccounts.Select(account => new AccountDto
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

    subscriptions = orderBy.ToLower() switch
    {
        "username" => subscriptions.OrderBy(s => s.Username).ToList(),
        "city" => subscriptions.OrderBy(s => s.City).ToList(),
        _ => subscriptions.OrderBy(s => s.Id).ToList(),
    };

    var total = subscriptions.Count;
    var totalPages = (int)Math.Ceiling((double)total / size);
    var paginatedSubscribers = subscriptions
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

[HttpGet("{id:int}/subscribers")]
[EnableCors("AllowFrontend")]
public async Task<IActionResult> GetSubscribersById(
    [FromRoute] int id,
    [FromQuery] string? stack = null,
    [FromQuery] string? firstLastName = null,
    [FromQuery] string? city = null,
    [FromQuery] string? orderBy = "id",
    [FromQuery] int page = 1,
    [FromQuery] int size = 50)
{
    var account = await _accountRepo.GetByIdAsync(id);

    if (account == null)
    {
        return NotFound("Account not found.");
    }

    var subscribersUsernames = account.subscribers;

   var subscriberAccounts = new List<Account>();

foreach (var username in subscribersUsernames)
{
    var subscriber = await _accountRepo.GetByUsernameAsync(username);
    if (subscriber != null)
    {
        subscriberAccounts.Add(subscriber);
    }
}

    var validSubscriberAccounts = subscriberAccounts.Where(subscriber => subscriber != null).ToList();
    if (!string.IsNullOrEmpty(stack))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(subscriber => subscriber.stack.Contains(stack))
            .ToList();
    }

    if (!string.IsNullOrEmpty(firstLastName))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(subscriber => (subscriber.firstName + " " + subscriber.lastName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase) ||
                              (subscriber.lastName + " " + subscriber.firstName).Contains(firstLastName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    if (!string.IsNullOrEmpty(city))
    {
        validSubscriberAccounts = validSubscriberAccounts
            .Where(subscriber => subscriber.city == city)
            .ToList();
    }

    var subscribers = validSubscriberAccounts.Select(subscriber => new AccountDto
    {
        Id = subscriber.Id,
        Username = subscriber.username,
        AvatarUrl = subscriber.avatarUrl,
        SubscribersAmount = subscriber.subscribersAmmount,
        FirstName = subscriber.firstName,
        LastName = subscriber.lastName,
        IsActive = subscriber.isActive,
        Stack = subscriber.stack,
        City = subscriber.city,
        Description = subscriber.description
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
[HttpPost("me/upload-image")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UploadImage(IFormFile file)
{
    DotEnv.Load();

    string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
    string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
    string regionCode = Environment.GetEnvironmentVariable("AWS_REGION");
    
    Amazon.RegionEndpoint region = Amazon.RegionEndpoint.GetBySystemName(regionCode);
    string bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");

    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    var account = await _accountRepo.GetAccountFromTokenAsync(token);

    if (account == null)
    {
        return NotFound("Account not found.");
    }

    if (file == null || file.Length == 0)
    {
        return BadRequest("File is not provided or empty.");
    }

    try
    {
        using var client = new AmazonS3Client(accessKey, secretKey, region);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = file.OpenReadStream(),
            Key = $"{Guid.NewGuid()}_{file.FileName}", // Generate unique file name
            BucketName = bucketName,
            ContentType = file.ContentType,
            CannedACL = S3CannedACL.PublicRead // Make the file publicly accessible
        };

        var transferUtility = new TransferUtility(client);
        await transferUtility.UploadAsync(uploadRequest);

        var fileUrl = $"https://{bucketName}.s3.{region.SystemName}.amazonaws.com/{uploadRequest.Key}";

        await _accountRepo.UploadImage(fileUrl, account.Id);
        return Ok(account);
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
    }
}

}