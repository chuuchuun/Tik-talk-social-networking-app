using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tik_talk.Data;
using tik_talk.Dtos;
using tik_talk.Interfaces;
using tik_talk.Models;

namespace tik_talk.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDBContext _context;

    public ChatRepository(ApplicationDBContext context)
    {
        _context = context;
    }
public async Task<Chat?> CreateAsync(Account user1, Account user2)
{
    if (user1 == null || user2 == null)
    {
        throw new ArgumentNullException("Both users must be valid.");
    }

    var chat = new Chat
    {
        userFirst = user1,
        userSecond = user2,
        userFirstId = user1.Id,
        userSecondId = user2.Id
    };

    user1.chatsAsFirstUser.Add(chat);  
    user2.chatsAsSecondUser.Add(chat);

    await _context.Chats.AddAsync(chat);
    await _context.SaveChangesAsync();

    return chat;
}



  public async Task<Chat?> DeleteAsync(int id)
  {
    var chatToDelete = await _context.Chats.FirstOrDefaultAsync(c => c.id == id);
    if(chatToDelete == null){
      return null;
    }
    var accountFirst = chatToDelete.userFirst;
    var accountSecond = chatToDelete.userSecond;

    accountFirst.chatsAsFirstUser.Remove(chatToDelete);
    accountSecond.chatsAsSecondUser.Remove(chatToDelete);
    _context.Chats.Remove(chatToDelete);
    await _context.SaveChangesAsync();

    return chatToDelete;
  }

  public async Task<List<Chat>> GetAllAsync()
  {
    return await _context.Chats.ToListAsync();
  }

  public Task<Chat?> GetByIdAsync(int chat_id)
  {
    throw new NotImplementedException();
  }

  
}
