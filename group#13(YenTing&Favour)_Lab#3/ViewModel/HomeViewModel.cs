using group_13_YenTing_Favour__Lab_3.Models;

namespace group_13_YenTing_Favour__Lab_3.ViewModel
{
    public class HomeViewModel
    {
        public List<Episode> MostPopularEpisodes { get; set; } = new();
        public List<Episode> RecentEpisodes { get; set; } = new();
        public List<Podcast> SearchResults { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
    }
}
