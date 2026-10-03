using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace forum.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        
        public List<Reply> Replies { get; set; } = new();
    }
}