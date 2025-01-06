using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Mappers;

public static class AccountMapper
{
    public static AccountDto ToAccountDto(this Account account){
            return new AccountDto{
                Id = account.Id,
                Username = account.username,
                AvatarUrl = account.avatarUrl,
                FirstName = account.firstName,
                LastName = account.lastName,
                Stack = account.stack,
                SubscribersAmount = account.subscribersAmmount,
                City = account.city,
                Description = account.description,
                IsActive =account.isActive
                };
        }

}
