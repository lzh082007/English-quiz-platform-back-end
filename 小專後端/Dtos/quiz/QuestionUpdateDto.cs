namespace 小專後端.Dtos.quiz
{
    public class QuestionUpdateDto
    {
        public int qid { get; set; }
        public string answer { get; set; }      
        public string content { get; set; }       
        public int categories_id { get; set; }   
        public byte difficulty_level { get; set; } 
    }
}
