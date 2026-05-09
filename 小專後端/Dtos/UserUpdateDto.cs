using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class UserUpdateDto
    {

        [Required(ErrorMessage = "舊密碼不能為空")]
        [DisplayName("密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字")]
        public string password{ get; set; } = null!;
        [Required(ErrorMessage = "新密碼不能為空")]
        [DisplayName("新密碼密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字")]
        public string New_password { get; set; } = null!;

        [Required(ErrorMessage = "確認密碼不能為空")]
        [DisplayName("確認密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字元")]
        public string _password { get; set; } = null!;
    }
}
