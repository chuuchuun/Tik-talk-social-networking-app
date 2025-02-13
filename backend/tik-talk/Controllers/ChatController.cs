using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tik_talk.Data;
using tik_talk.Dtos;
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
        var chat = await _chatRepo.CreateAsync(currentAccount.Id, secondUser.Id);

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

    [Authorize]
    [HttpGet("get_my_chats")]
    [EnableCors("AllowFrontend")] 

    public async Task<IActionResult> GetMyChats()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        try
        {
            var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);
            var myId = myAccount.Id;
            var chats = await _chatRepo.GetAllAsync();
            var myChats = chats.Where(chat => chat.userFirst== myId || chat.userSecond == myId).ToList();
            List<ChatReadDto> chatDtos = new List<ChatReadDto>();
            foreach(Chat chat in chats){
                chatDtos.Add(new ChatReadDto{
                    id = chat.id,
                    userFirst = chat.userFirst,
                    userSecond = chat.userSecond,
                    messages = chat.messages
                });
            }
            
            return Ok(chatDtos);
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
}
