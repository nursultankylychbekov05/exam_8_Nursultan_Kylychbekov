using Microsoft.AspNetCore.Identity;

namespace forum.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; } = "/images/default-avatar.png";
        public DateTime BirthDate { get; set; }
        
        public List<Topic> Topics { get; set; } = new();
        public List<Reply> Replies { get; set; } = new();
    }
}