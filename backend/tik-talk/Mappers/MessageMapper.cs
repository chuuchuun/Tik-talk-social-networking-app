using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;
using tik_talk.Models;

namespace tik_talk.Mappers;

public static class MessageMapper
{
    public static MessageReadDto ToMessageDto(this Message message){
            return new MessageReadDto{
                id = message.id,
                text = message.text,
                userFromId = message.userFromId,
                createdAt = message.createdAt
                };
    }
}
