using System.ComponentModel;

namespace 小專後端.Dtos.ticket
{
    public class ProblemDto
    {
        [DisplayName("回報問題的使用者ID")]
        public int reporter_id { get; set; }
        [DisplayName("問題ID")]
        public int? question_id { get; set; }
        [DisplayName("使用者填寫的問題描述")]
        public String description { get; set; }
        [DisplayName("問題單處理狀態")]
        public bool status { get; set; }
        [DisplayName("問題類別")]
        public String problem_type { get; set; }

    }
}
