using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tik_talk.Models;

public class Comment
{
    public int id{ get; set; }
    public required string text { get; set; }
    public required int authorId { get; set; }
    public required int postId { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}
