using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Models;

namespace tik_talk.Dtos;

public class ChatReadDto
{
    public int id { get; set; }
    public required int userFirst { get; set; }
    public required int userSecond { get; set; }
    public string lastMessage { get; set; } = String.Empty;
    public List<MessageReadDto> messages { get; set; } = new List<MessageReadDto>();
}
