using System.ComponentModel;

namespace 小專後端.Dtos.ticket
{
    public class AdminProblemListDto
    {
        [DisplayName("問題單ID")]
        public int Pid { get; set; }
        [DisplayName("使用者gmail")]
        public string Email { get; set; }
        [DisplayName("問題單內描述")]
        public string Description { get; set; }
        [DisplayName("問題類別")]
        public string QuestionType { get; set; }
        [DisplayName("問題單狀態")]
        public bool Status { get; set; }
        [DisplayName("問題單創建時間")]
        public DateTime? CreatedAt { get; set; } 
    }
}
