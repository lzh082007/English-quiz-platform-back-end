using System.ComponentModel;

namespace 小專後端.Dtos.quiz
{
    public class QuizResponseDto
    {
        public int QuestionId { get; set; }
        [DisplayName("題目內容")]
        public string QuestionContent { get; set; } 
        [DisplayName("使用者當時選的答案")]
        public string UserAnswer { get; set; }
        [DisplayName("真正的正確答案")]
        public string CorrectAnswer { get; set; }
        [DisplayName("標記對錯")]
        public bool IsWrong { get; set; }
        [DisplayName("錯誤選項")]
        public List<string> ErrorOptions { get; set; }
    }
}
