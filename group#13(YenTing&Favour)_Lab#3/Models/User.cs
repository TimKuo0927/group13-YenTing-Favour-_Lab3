using System;
using System.Collections.Generic;

namespace group_13_YenTing_Favour__Lab_3.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Podcast> Podcasts { get; set; } = new List<Podcast>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
