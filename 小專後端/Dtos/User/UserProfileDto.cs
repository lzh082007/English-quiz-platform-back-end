namespace 小專後端.Dtos.User
{
    public class UserProfileDto
    {
        public int uid { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public DateTime? created_at { get; set; }
        public string role { get; set; }
    }
}
