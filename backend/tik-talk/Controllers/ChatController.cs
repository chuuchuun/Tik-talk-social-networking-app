using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tik_talk.Data;
using tik_talk.Interfaces;
using tik_talk.Mappers;
using tik_talk.Models;

namespace tik_talk.Controllers;

[Route("api/chat")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{

    private readonly ApplicationDBContext _context;
    private readonly IAccountRepository _accountRepo;
    private readonly ITokenService _tokenservice;
    private readonly IChatRepository _chatRepo;
    public ChatController(ApplicationDBContext context, IChatRepository chatRepo, IAccountRepository accountRepository, ITokenService tokenService){
        _context = context;
        _accountRepo = accountRepository;
        _tokenservice = tokenService;
        _chatRepo = chatRepo;
    }

    [HttpPost]
    [Authorize]
    [Route("{user_id:int}")]
    public async Task<IActionResult> CreateChat([FromRoute] int user_id){
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        var currentAccount = await _accountRepo.GetAccountFromTokenAsync(token);

        if (currentAccount == null)
        {
            Console.WriteLine("No account found for the token.");
            return NotFound("Current account not found.");
        }

        var secondUser = await  _accountRepo.GetByIdAsync(user_id);
        var chat = await _chatRepo.CreateAsync(currentAccount, secondUser);

        return Ok(chat);
    }

    [HttpDelete]
[Route("{id:int}")]
public async Task<IActionResult> DeleteChat(int id)
{
    var chat = await _chatRepo.DeleteAsync(id);


    return NoContent();
}

    [HttpGet]
    public async Task<IActionResult> GetAllChats(){
        var chats = await _chatRepo.GetAllAsync();
        return Ok(chats);
    }
}
