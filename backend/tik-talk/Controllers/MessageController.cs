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
public async Task<IActionResult> SendMessage([FromRoute] int chat_id, [FromBody] string message)
{
    var chat = await _chatRepo.GetByIdAsync(chat_id);
    if (chat == null)
    {
        return NotFound($"Chat with ID {chat_id} not found.");
    }

    var wholeMessage = new Message
    {
        chatId = chat_id,
        message = message
    };

    await _chatRepo.SendMessage(wholeMessage);
    return Ok();
}

}
