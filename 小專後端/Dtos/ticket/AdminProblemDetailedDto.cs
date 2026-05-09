using System.ComponentModel;

namespace 小專後端.Dtos.ticket
{
    public class AdminProblemDetailedDto
    {
        [DisplayName("問題單ID")]
        public int Pid { get; set; }
        [DisplayName("使用者gmail")]
        public string Email { get; set; }
        [DisplayName("問題單內詳細資料")]
        public string Description { get; set; }
        [DisplayName("問題單問題類別")]
        public string QuestionType { get; set; }
        [DisplayName("問題單狀態")]
        public bool Status { get; set; }
        [DisplayName("問題單創建時間")]
        public DateTime? CreatedAt { get; set; }

        //如果是題目錯誤且question_id有值，這裡就會帶入下方ProblemDto的資料；系統bug則為null
        public AdminProblemQustion QuestionInfo { get; set; }
    }
}
