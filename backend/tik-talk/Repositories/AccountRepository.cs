using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tik_talk.Data;
using tik_talk.Dtos;
using tik_talk.Interfaces;
using tik_talk.Mappers;
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

public async Task<bool> SubscribeAsync(int id1, int id2)
{
    var subscription = await _context.Accounts
        .FirstOrDefaultAsync(a => a.Id == id2);
    var subscriber = await _context.Accounts
        .FirstOrDefaultAsync(a => a.Id == id1);
        
    if (subscription == null)
    {
        return false;
    }
    if (subscriber == null)
    {
        return false;
    }

    try
    {
        
        subscription.subscribers.Add(subscriber.username);
        // Add the subscription to the subscriber's list
        subscriber.subscriptions.Add(subscription.username);

        // Save changes to the database
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        throw(ex);
        // Optionally log the exception for debugging
        // _logger.LogError(ex, "Error occurred while subscribing.");
        return false;
    }
}

 public async Task<Account> GetAccountFromTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("Token is missing.");
        }

        // Decode the token and extract claims
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new ArgumentException("Invalid token format.");
        }

        // Extract the 'given_name' claim
        var givenName = jsonToken?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

        if (string.IsNullOrEmpty(givenName))
        {
            throw new UnauthorizedAccessException("Given name not found in the token.");
        }

        // Retrieve the account by username (givenName)
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.username == givenName);

        if (account == null)
        {
            throw new UnauthorizedAccessException("Account not found.");
        }

        return account;
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
