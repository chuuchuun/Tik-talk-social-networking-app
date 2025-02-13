using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Interfaces;

public interface IChatRepository
{
    Task<List<Chat>> GetAllAsync();
    Task<Chat?> CreateAsync(int user1, int user2);
    Task<Chat?> GetByIdAsync(int chat_id);
    Task<Chat?> DeleteAsync(int id);
    Task<Chat> SendMessage(Message message);
}
