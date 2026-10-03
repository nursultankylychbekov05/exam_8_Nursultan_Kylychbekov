namespace forum.Models
{
    public class ReplyVote
    {
        public int Id { get; set; }
        
        public int ReplyId { get; set; }
        public Reply Reply { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public bool IsLike { get; set; } 
    }
}