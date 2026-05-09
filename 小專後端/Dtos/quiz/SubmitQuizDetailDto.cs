namespace 小專後端.Dtos.quiz
{
    public class SubmitQuizDetailDto
    {
        public int QuestionId { get; set; }
        public bool IsWrong { get; set; }
        public string UserAnswer { get; set; }
        public string Error_A { get; set; }
        public string Error_B { get; set; }
        public string Error_C { get; set; }

    }
}
