using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tik_talk.Data;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Controllers;
[Route("api/message")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IAccountRepository _accountRepo;
    private readonly ITokenService _tokenservice;
    private readonly IChatRepository _chatRepo;

    public MessageController(IChatRepository chatRepo, ApplicationDBContext context, IAccountRepository accountRepository, ITokenService tokenService){
        _context = context;
        _accountRepo = accountRepository;
        _tokenservice = tokenService;
        _chatRepo = chatRepo;
    }
    
   [HttpPost]
[Authorize]
[Route("{chat_id:int}")]
public async Task<IActionResult> SendMessage([FromRoute] int chat_id, [FromQuery] string text)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    try
    {
        // Debug: Log the token
        Console.WriteLine($"Extracted Token: {token}");

        var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);
        if (myAccount == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var myId = myAccount.Id;

        var chat = await _chatRepo.GetByIdAsync(chat_id);
        if (chat == null)
        {
            return NotFound($"Chat with ID {chat_id} not found.");
        }

        var wholeMessage = new Message
        {
            chatId = chat_id,
            userFromId = myId,
            text = text,
            createdAt = DateTime.UtcNow
        };

        await _chatRepo.SendMessage(wholeMessage);
        return Ok(wholeMessage);
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized("Unauthorized access.");
    }
    catch (Exception ex)
    {
        // Log the actual exception message
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
 [HttpDelete]
[Authorize]
[Route("{chat_id:int}/{message_id:int}")]
public async Task<IActionResult> DeleteMessage([FromRoute] int chat_id, [FromRoute] int message_id)
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
    try
    {
        var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);
        if (myAccount == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var myId = myAccount.Id;

        var chat = await _chatRepo.DeleteMessageByIdAsync(message_id);
        return Ok(chat);
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized("Unauthorized access.");
    }
    catch (Exception ex)
    {
        // Log the actual exception message
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}




}
