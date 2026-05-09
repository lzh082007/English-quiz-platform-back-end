using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class personal_lists
{
    public int lid { get; set; }

    public int user_id { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<list_items> list_items { get; set; } = new List<list_items>();

    public virtual users user { get; set; } = null!;
}
