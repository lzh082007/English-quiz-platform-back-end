using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class ChangePwdDto
    {
        [Required(ErrorMessage = "密碼不能為空")]
        [DisplayName("舊密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到100個字元")]
        public string oldPassword { get; set; } = null!;
        [Required(ErrorMessage = "密碼不能為空")]
        [DisplayName("新密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到100個字元")]
        public string newPassword { get; set; } = null!;
        [Required(ErrorMessage = "密碼不能為空")]
        [DisplayName("確認密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到100個字元")]
        [Compare("newPassword", ErrorMessage = "兩次輸入的密碼不一致")]
        public string againNewPassword { get; set; } = null!;
    }
}
