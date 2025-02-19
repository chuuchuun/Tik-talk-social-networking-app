using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account> CreateAsync(Account accountModel);
    Task<Account?> UpdateAsync(int id, UpdateAccount accountModel);
    Task<Account?> DeleteAsync(int id);
    Task<Account?> GetByUsernameAsync(string username);
    Task<bool> SubscribeAsync(int id1, int id2);
    Task<Account> GetAccountFromTokenAsync(string token);
    Task<Account?> UploadImage(string path,int id);
}
