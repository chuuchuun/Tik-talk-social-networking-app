using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tik_talk.Dtos;

namespace tik_talk.Models;

public class Chat
{
    public int id {get;set;}
    public int userFirstId { get; set; } // Foreign Key for UserFirst
    public int userSecondId { get; set; } // Foreign Key for UserSecond
        public Account userFirst {get;set;}
    public Account userSecond{get;set;} 
    public List<string> messages{get;set;} = new List<string>();
}