using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class UpdateAccount
{
    public string firstName { get; set; } = string.Empty;
    public string lastName { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public List<string> stack { get; set; } = new List<string> ();
}
