using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class categories
{
    public int cid { get; set; }

    public string categories1 { get; set; } = null!;

    public virtual ICollection<words> words { get; set; } = new List<words>();
}
