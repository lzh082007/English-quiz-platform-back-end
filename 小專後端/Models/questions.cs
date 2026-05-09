using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class questions
{
    public int qid { get; set; }

    public int word_id { get; set; }

    public int question_error { get; set; }

    public int question_correct { get; set; }

    public int question_type_id { get; set; }

    public string question_content { get; set; } = null!;
    public bool IsDeleted { get; set; }

    public virtual ICollection<problem_list> problem_list { get; set; } = new List<problem_list>();

    public virtual question_type question_type { get; set; } = null!;

    public virtual ICollection<quiz_detail_logs> quiz_detail_logs { get; set; } = new List<quiz_detail_logs>();

    public virtual words word { get; set; } = null!;
}
