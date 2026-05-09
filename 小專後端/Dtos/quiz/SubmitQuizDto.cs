namespace 小專後端.Dtos.quiz
{
    public class SubmitQuizDto
    {
        public byte Source { get; set; } 

        public TimeSpan TotalTimeSpent { get; set; } //總共花費時間
        public int CorrectCount { get; set; } //答對題數
        public int? ListId { get; set; } //答對題數
        public int WrongCount { get; set; }   //答錯題數
        public List<SubmitQuizDetailDto> Details { get; set; } //每一題的明細

    }
}
