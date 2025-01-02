using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Models;

[Table("Accounts")]
public class Account
{
    public int Id {get;set;}
    public string username {get;set;}
    public string avatarUrl{get;set;} = string.Empty;
    public int subscribersAmmount{get;set;}
    public string firstName {get;set;}
    public string lastName {get;set;}
    public bool isActive {get;set;}
    public List<string> stack {get;set;} = new List<string>();
    public string city{get;set;}
    public string description{get;set;} = string.Empty;
}
