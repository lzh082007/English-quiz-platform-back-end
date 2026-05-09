using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class login_time
{
    public int user_id { get; set; }

    public DateTime login_at { get; set; }

    public virtual users user { get; set; } = null!;
}
