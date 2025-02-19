using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;

namespace tik_talk.Models;

public class Chat
{
    public int id {get;set;}
    public int userFirst {get;set;}
    public int userSecond{get;set;} 
    public List<Message> messages{get;set;} = new List<Message>();
}