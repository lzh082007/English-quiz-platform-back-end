using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class quiz_detail_logs
{
    public int quiz_record_id { get; set; }

    public int question_id { get; set; }

    public bool is_wrong { get; set; }

    public string user_answer { get; set; } = null!;

    public TimeOnly time_taken { get; set; }

    public virtual questions question { get; set; } = null!;

    public virtual ICollection<quiz_error_option> quiz_error_optionquiz_detail_logs { get; set; } = new List<quiz_error_option>();

    public virtual ICollection<quiz_error_option> quiz_error_optionquiz_detail_logsNavigation { get; set; } = new List<quiz_error_option>();

    public virtual quiz_records quiz_record { get; set; } = null!;
}
