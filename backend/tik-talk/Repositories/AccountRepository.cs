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

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDBContext _context;
    public AccountRepository(ApplicationDBContext context){
        _context = context;
    }
    public async Task<Account> CreateAsync(Account accountModel)
    {
        await _context.Accounts.AddAsync(accountModel);
        await _context.SaveChangesAsync();
        return accountModel;
    }

    public async Task<Account?> DeleteAsync(int id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if(account == null){
            return null;
        }
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return account;    
    }

    public async Task<List<Account>> GetAllAsync()
    {
    
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await _context.Accounts.FirstOrDefaultAsync(i => i.Id == id);
   
    }
public async Task<Account?> GetByUsernameAsync(string username)
{
    return await _context.Accounts
        .FirstOrDefaultAsync(a => a.username == username);
}

  public async Task<Account?> UpdateAsync(int id, Account accountModel)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if(account == null){
            return null;
        }   
        account.username = accountModel.username;    
        account.avatarUrl = accountModel.avatarUrl;
        account.firstName = accountModel.firstName;
        account.lastName = accountModel.lastName;
        account.description =  accountModel.description;
        account.city = accountModel.city;
        account.isActive = accountModel.isActive;
        account.stack = accountModel.stack;
        account.subscribersAmmount = account.subscribersAmmount;
        await _context.SaveChangesAsync(); 
        return account; 
    }
}
