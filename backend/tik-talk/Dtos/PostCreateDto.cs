using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class PostCreateDto
{
    public string? content { get; set; } = string.Empty;
    public int authorId { get; set; }
}
