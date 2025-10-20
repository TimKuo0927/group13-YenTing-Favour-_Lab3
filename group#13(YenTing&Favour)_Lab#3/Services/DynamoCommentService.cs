using Amazon.DynamoDBv2.DocumentModel;
using group_13_YenTing_Favour__Lab_3.Models;

namespace group_13_YenTing_Favour__Lab_3.Services
{
    public class DynamoCommentService
    {
        private readonly Table _commentTable;

        public DynamoCommentService()
        {
            _commentTable = AWSUtil.userTable; // your DynamoDB table loaded in AWSUtil
        }

        // Add comment
        public async Task AddCommentAsync(Comment comment)
        {
            var doc = new Document
            {
                ["EpisodeId"] = comment.EpisodeId,
                ["CommentId"] = comment.CommentId,
                ["UserName"] = comment.UserName,
                ["Message"] = comment.Message,
                ["CreatedAt"] = comment.CreatedAt.ToString("o")
            };
            await _commentTable.PutItemAsync(doc);
        }

        // Get all comments for an episode
        public async Task<List<Comment>> GetCommentsByEpisodeAsync(int episodeId)
        {
            var queryConfig = new QueryOperationConfig
            {
                KeyExpression = new Expression
                {
                    ExpressionStatement = "EpisodeId = :v_id",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":v_id", episodeId }
                    }
                },
                BackwardSearch = true // newest first
            };

            var search = _commentTable.Query(queryConfig);
            var results = new List<Comment>();

            do
            {
                var set = await search.GetNextSetAsync();
                results.AddRange(set.Select(doc => new Comment
                {
                    EpisodeId = Convert.ToInt32(doc["EpisodeId"]),
                    CommentId = doc["CommentId"],
                    UserName = doc["UserName"],
                    Message = doc["Message"],
                    CreatedAt = DateTime.Parse(doc["CreatedAt"])
                }));
            }
            while (!search.IsDone);

            return results.OrderByDescending(c => c.CreatedAt).ToList();
        }

        // Delete comment
        public async Task DeleteCommentAsync(int episodeId, string commentId)
        {
            await _commentTable.DeleteItemAsync(episodeId, commentId);
        }

        // Update an existing comment's message
        public async Task UpdateCommentAsync(int episodeId, string commentId, string newMessage)
        {
            var updateDoc = new Document
            {
                ["EpisodeId"] = episodeId,
                ["CommentId"] = commentId,
                ["Message"] = newMessage,
                ["UpdatedAt"] = DateTime.UtcNow.ToString("o")
            };

            // Use UpdateItemAsync so we only modify certain fields
            var updateItemRequest = new UpdateItemOperationConfig
            {
                ReturnValues = ReturnValues.AllNewAttributes
            };

            var existing = await _commentTable.GetItemAsync(episodeId, commentId);
            if (existing == null)
                throw new Exception("Comment not found");

            existing["Message"] = newMessage;
            existing["UpdatedAt"] = DateTime.UtcNow.ToString("o");

            await _commentTable.PutItemAsync(existing);
        }

    }
}
