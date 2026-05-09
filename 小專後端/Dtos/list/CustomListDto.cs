namespace 小專後端.Dtos.list
{
    public class CustomListDto
    {
        public int ListId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int WordCount { get; set; }  
        public DateTime? LastUpdated { get; set; } 
    }
}
