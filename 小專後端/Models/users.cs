using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class users
{
    public int uid { get; set; }

    public string email { get; set; } = null!;

    public string password { get; set; } = null!;

    public string nickname { get; set; } = null!;

    public string? auth_code { get; set; }

    public string role { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public virtual ICollection<personal_lists> personal_lists { get; set; } = new List<personal_lists>();

    public virtual ICollection<problem_list> problem_list { get; set; } = new List<problem_list>();

    public virtual ICollection<quiz_records> quiz_records { get; set; } = new List<quiz_records>();
}
