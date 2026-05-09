namespace 小專後端.Dtos
{
    public class QuestionListDto
    {
        public int qid { get; set; }
        public int? word_id { get; set; }
        public string question_content { get; set; }
        public int? question_type_id { get; set; }
        public string spelling { get; set; }
        public byte? difficulty_level { get; set; }
        public string CategoryName { get; set; }
    }
}
