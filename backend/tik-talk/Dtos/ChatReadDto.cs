using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class ChatReadDto
{
    public int id { get; set; }
    public required int userFirst { get; set; }
    public required int userSecond { get; set; }
    public List<string> messages { get; set; } = new List<string>();
}
