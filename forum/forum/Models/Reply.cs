using System;
using System.ComponentModel.DataAnnotations;

namespace forum.Models
{
    public class Reply
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}