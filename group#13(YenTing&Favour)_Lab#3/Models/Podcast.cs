using System;
using System.Collections.Generic;

namespace group_13_YenTing_Favour__Lab_3.Models;

public partial class Podcast
{
    public int PodcastId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid CreatorId { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsHidden { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
