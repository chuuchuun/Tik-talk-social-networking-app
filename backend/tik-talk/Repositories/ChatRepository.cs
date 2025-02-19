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
public async Task<Chat?> CreateAsync(int user1, int user2)
{ 
    if (user1 == null || user2 == null)
    {
        throw new ArgumentNullException("Both users must be valid.");
    }

    var chat = new Chat
    {
        userFirst = user1,
        userSecond = user2,
    };

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

    _context.Chats.Remove(chatToDelete);
    await _context.SaveChangesAsync();

    return chatToDelete;
  }

  public async Task<List<Chat>> GetAllAsync()
  {
    return await _context.Chats.Include(c=>c.messages).ToListAsync();
  }

  public async Task<Chat?> GetByIdAsync(int chat_id)
  {
    return await _context.Chats.Include(c=> c.messages).FirstOrDefaultAsync(c => c.id == chat_id);
  }

public async Task<Chat> SendMessage(Message message)
{
    var chat = await _context.Chats
        .Include(c => c.messages)  // Ensure messages are loaded
        .FirstOrDefaultAsync(c => c.id == message.chatId);

    if (chat == null)
    {
        throw new Exception($"Chat with ID {message.chatId} not found.");
    }

    // Explicitly add the message to the Messages table
    _context.Messages.Add(message);
    chat.messages.Add(message);
    await _context.SaveChangesAsync();
    return chat;
}


}
