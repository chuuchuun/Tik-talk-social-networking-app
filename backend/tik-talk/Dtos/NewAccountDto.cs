using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class NewAccountDto
{
    
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public int SubscribersAmount { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public List<string> Stack { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
}
