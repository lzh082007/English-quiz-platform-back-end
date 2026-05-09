namespace 小專後端.Dtos
{
    public class WordListDto
    {
        public int wid { get; set; }
        public string spelling { get; set; } = null!;
        public string meaning { get; set; } = null!;
        public string parts_of_speech { get; set; } = null!;
        public string KK { get; set; } = null!;
        public string Example { get; set; }
        public byte difficulty_level { get; set; }

        public int categories_id { get; set; }
        public string CategoryName { get; set; } = null!; 
    }
}
