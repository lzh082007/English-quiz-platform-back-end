using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "電子郵件不能為空")]
        [EmailAddress(ErrorMessage ="電子郵件格式不正確")]
        [DisplayName("電子郵件")]
        public string email{ get; set; } = null!;

        [Required(ErrorMessage = "密碼不能為空")]
        [DisplayName("密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字")]
        public string password{ get; set; } = null!;
        [Required(ErrorMessage = "密碼不能為空")]
        [DisplayName("確認密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字元")]
        [Compare("password", ErrorMessage = "兩次輸入的密碼不一致")]
        public string againpassword { get; set; } = null!;

        [Required(ErrorMessage = "暱稱不能為空")]
        [DisplayName("暱稱")]
        [StringLength(50, ErrorMessage = "暱稱長度不能超過50個字元")]
        public string nickname{ get; set; } = null!;
    }
}
