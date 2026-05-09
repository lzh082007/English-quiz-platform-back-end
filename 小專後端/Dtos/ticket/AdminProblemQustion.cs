using System.ComponentModel;

namespace 小專後端.Dtos.ticket
{
    public class AdminProblemQustion
    {
        [DisplayName("問題ID")]
        public int Qid { get; set; }
        [DisplayName("單字ID")]
        public int WordId { get; set; }
        [DisplayName("問題內容")]
        public string QuestionContent { get; set; }

        // 如果維護人員在後台修改題目時，連同答案也要一起改，這兩個欄位可以順便拉出來給前端
        [DisplayName("題目內對的答案")]
        public int QuestionCorrect { get; set; }
        [DisplayName("題目內錯的答案")]
        public int QuestionError { get; set; }    
    }
}
