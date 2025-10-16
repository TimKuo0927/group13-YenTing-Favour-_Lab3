using System;
using System.Collections.Generic;

namespace group_13_YenTing_Favour__Lab_3.Models;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string UserId { get; set; }

    public int PodcastId { get; set; }

    public DateTime SubscribedDate { get; set; }

    public virtual Podcast Podcast { get; set; } = null!;

    //public virtual User User { get; set; } = null!;
}
