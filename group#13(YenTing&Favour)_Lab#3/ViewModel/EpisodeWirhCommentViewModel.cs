using group_13_YenTing_Favour__Lab_3.Models;

namespace group_13_YenTing_Favour__Lab_3.ViewModel
{
    public class EpisodeWirhCommentViewModel
    {
        public int EpisodeId { get; set; }

        public int PodcastId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime ReleaseDate { get; set; }

        public double Duration { get; set; }

        public int PlayCount { get; set; }

        public string AudioFileUrl { get; set; } = null!;

        public int NumberOfViews { get; set; }

        public virtual Podcast Podcast { get; set; } = null!;

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
