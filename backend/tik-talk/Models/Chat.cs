using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;

namespace tik_talk.Models;

public class Chat
{
    public int Id {get;set;}
    public AccountDto userFirst {get;set;}
    public AccountDto userSecond{get;set;} 
    public List<string> messages{get;set;} = new List<string>();
}