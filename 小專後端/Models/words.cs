using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class words
{
    public int wid { get; set; }

    public string spelling { get; set; } = null!;

    public string meaning { get; set; } = null!;

    public string parts_of_speech { get; set; } = null!;

    public string Example { get; set; } = null!;

    public string KK { get; set; } = null!;

    public int categories_id { get; set; }

    public byte difficulty_level { get; set; }

    public virtual categories categories { get; set; } = null!;

    public virtual ICollection<list_items> list_items { get; set; } = new List<list_items>();

    public virtual ICollection<questions> questions { get; set; } = new List<questions>();

    public virtual ICollection<quiz_error_option> quiz_error_option { get; set; } = new List<quiz_error_option>();
}
