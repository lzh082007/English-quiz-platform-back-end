using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class UserReadDto
    {
        [DisplayName("使用者編號")]
        public int uid { get; set; }
        [DisplayName("電子郵件")]
        public string email{ get; set; } = null!;

        [Required(ErrorMessage = "暱稱不能為空")]
        [DisplayName("暱稱")]
        [StringLength(50, ErrorMessage = "暱稱長度不能超過50個字元")]
        public string nickname{ get; set; } = null!;

        [Required(ErrorMessage = "角色")]
        [StringLength(20, ErrorMessage = "角色長度不能超過20個字元")]
        public string role{ get; set; } = null!;
        [DisplayName("創建時間")]
        public DateTime Created_at { get; set; }
    }
}
