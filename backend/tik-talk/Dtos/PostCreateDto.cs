using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Dtos;

public class PostCreateDto
{
    public required string title { get; set; } = string.Empty;
    public string? content { get; set; } = string.Empty;
    public int authorId { get; set; }


}
