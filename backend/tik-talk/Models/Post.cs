using System;
using System.Collections.Generic;

namespace tik_talk.Models
{
    public class Post
    {
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public int communityId { get; set; }
        public string content { get; set; } = string.Empty;
        public int authorId { get; set; } 
        
        public List<string> images { get; set; } = new List<string>();
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int likes { get; set; }
        public List<string> comments { get; set; } = new List<string>();
    }
}
