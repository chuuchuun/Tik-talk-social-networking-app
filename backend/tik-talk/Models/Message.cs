using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Models;

public class Message
{
    public int id { get; set; }
    public int chatId { get; set; }
    public Chat Chat { get; set; }
    public int userFromId {get;set;}
    public required string text { get; set; }
    public DateTime createdAt { get; set; }
}
