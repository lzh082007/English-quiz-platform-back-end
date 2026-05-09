using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class Userforgetpassword
    {

        public string uid { get; set; } = null!;
        public string auth_code { get; set; } = null!;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字")]
        public string New_password { get; set; } = null!;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度需介於6到20個字")]
        public string confirm_password { get; set; } = null!;
    }
}
