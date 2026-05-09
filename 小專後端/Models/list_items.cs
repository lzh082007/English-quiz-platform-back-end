using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class list_items
{
    public int list_id { get; set; }

    public int word_id { get; set; }

    public DateTime? created_at { get; set; }

    public virtual personal_lists list { get; set; } = null!;

    public virtual words word { get; set; } = null!;
}
