namespace 小專後端.Dtos.Quiz
{
    public class QuizQuestionDto
    {
        public int QuestionId { get; set; } 
        public string QuestionContent { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string Answer { get; set; }
    }
}
