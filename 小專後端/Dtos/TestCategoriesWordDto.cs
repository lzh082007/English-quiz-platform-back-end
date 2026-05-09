using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class TestWordDto
    {
        [Required(ErrorMessage = "分類編號不能為空")]
        [DisplayName("分類編號")]
        public int categories_id { get; set; }

        [Required(ErrorMessage = "難度等級不能為空")]
        [Range(1,4, ErrorMessage = "難度等級只能介於1到4")]
        [DisplayName("難度等級")]
        public byte difficulty_level { get; set; }
    }
}
