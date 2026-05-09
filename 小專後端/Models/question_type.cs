using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class question_type
{
    public int tid { get; set; }

    public string question_type1 { get; set; } = null!;

    public virtual ICollection<questions> questions { get; set; } = new List<questions>();
}
