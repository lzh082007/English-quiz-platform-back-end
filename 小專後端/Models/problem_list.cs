using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class problem_list
{
    public int pid { get; set; }

    public int reporter_id { get; set; }

    public int? question_id { get; set; }

    public string description { get; set; } = null!;

    public bool status { get; set; }
    public string problem_type { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public virtual questions question { get; set; } = null!;

    public virtual users reporter { get; set; } = null!;
}
