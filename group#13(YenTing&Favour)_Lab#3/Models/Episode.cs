using System;
using System.Collections.Generic;

namespace group_13_YenTing_Favour__Lab_3.Models;

public partial class Episode
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
}
