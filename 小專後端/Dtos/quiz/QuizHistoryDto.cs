using System.ComponentModel;

namespace 小專後端.Dtos.quiz
{
    public class QuizHistoryDto
    {
        [DisplayName("總表ID")]
        public int RecordId { get; set; }
        [DisplayName("測驗來源")]
        public string SourceName { get; set; }
        [DisplayName("測驗日期")]
        public string QuizDate { get; set; }
        public int? ListId { get; set; } //答對題數
        [DisplayName("花費時間")]
        public string TotalTime { get; set; }
        [DisplayName("答對題數")]
        public int CorrectCount { get; set; } 
        [DisplayName("答錯題數")]
        public int WrongCount { get; set; }     
    }
}
