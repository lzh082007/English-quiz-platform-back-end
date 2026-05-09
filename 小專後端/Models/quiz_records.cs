using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class quiz_records
{
    public int id { get; set; }

    public int user_id { get; set; }
    public int? list_id { get; set; }

    public byte source { get; set; }

    public DateTime? quiz_at { get; set; }

    public TimeOnly total_time_spent { get; set; }

    public int correct_count { get; set; }

    public int wrong_count { get; set; }

    public virtual ICollection<quiz_detail_logs> quiz_detail_logs { get; set; } = new List<quiz_detail_logs>();

    public virtual users user { get; set; } = null!;
}
