using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class quiz_error_option
{
    public int quiz_record_id { get; set; }

    public int question_id { get; set; }

    public int option_word_id { get; set; }

    public virtual words option_word { get; set; } = null!;

    public virtual quiz_detail_logs quiz_detail_logs { get; set; } = null!;

    public virtual quiz_detail_logs quiz_detail_logsNavigation { get; set; } = null!;
}
