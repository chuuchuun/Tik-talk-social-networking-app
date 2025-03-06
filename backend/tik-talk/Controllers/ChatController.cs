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
    public async Task<IActionResult> GetMyChats(
        [FromQuery] string? username = null
    )
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        try
        {
            var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);
            var myId = myAccount.Id;

            var chats = await _chatRepo.GetAllAsync();

            // Filter only the chats where the user is involved
            var myChats = chats.Where(chat => chat.userFirst == myId || chat.userSecond == myId).ToList();

            if(!string.IsNullOrEmpty(username))
            {
                var user = await _accountRepo.GetByUsernameAsync(username.ToLower());
                if(user == null){
                    return BadRequest("User not found");
                }
                var userId = user.Id;
                myChats = myChats.Where(c => c.userFirst == userId || c.userSecond == userId).ToList();
            }
            if (!myChats.Any()) 
            {
                return Ok(new List<ChatReadDto>()); // Return an empty list if no chats exist
            }

            List<ChatReadDto> chatDtos = new List<ChatReadDto>();
            
            foreach (Chat chat in myChats)
            {
                List<MessageReadDto> msgDtos = new List<MessageReadDto>();
                foreach(Message msg in chat.messages){
                    msgDtos.Add(msg.ToMessageDto());
                }
                chatDtos.Add(new ChatReadDto
                {
                    id = chat.id,
                    userFirst = chat.userFirst,
                    userSecond = chat.userSecond,
                    lastMessage = chat.messages.Count > 0 ? chat.messages.Last().text : "No messages yet" ,// Prevent exception
                    messages = msgDtos
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

  

    [Authorize]
    [EnableCors("AllowFrontend")] 
    [HttpGet("{id:int}/messages")]
    public async Task<IActionResult> GetMessages(int id)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        try
        {
            var myAccount = await _accountRepo.GetAccountFromTokenAsync(token);
            var myId = myAccount.Id;

            var chat = await _chatRepo.GetByIdAsync(id);
           if(chat!=null){
            List<Message> messages = chat.messages;
            List<MessageReadDto> messagesDto = new List<MessageReadDto>();
            
            foreach (Message message in messages)
            {
                messagesDto.Add(new MessageReadDto
                {
                    id = message.id,
                    userFromId = message.userFromId,
                    text = message.text,
                    createdAt = message.createdAt
                });
            }
            
            return Ok(messagesDto);
        }
        else{
            return BadRequest("Chat not found");
        }
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
