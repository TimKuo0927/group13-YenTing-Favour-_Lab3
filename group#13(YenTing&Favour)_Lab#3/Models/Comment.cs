namespace group_13_YenTing_Favour__Lab_3.Models
{
    //for storing comments in DynamoDB
    public class Comment
    {
        public int EpisodeId { get; set; }
        public string CommentId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
