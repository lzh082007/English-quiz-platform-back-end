using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace 小專後端.Dtos
{
    public class WordUpdateDto
    {

        [DisplayName("英文單字編號ID")]
        public int wid { get; set; }

        [Required(ErrorMessage = "英文單字不能為空")]
        [DisplayName("英文單字")]
        [StringLength(50, ErrorMessage = "英文單字長度不能超過50字")]
        public string spelling { get; set; } = null!;

        [Required(ErrorMessage = "中文意思不能為空")]
        [DisplayName("中文意思")]
        [StringLength(255, ErrorMessage = "中文意思長度不能超過225個字元")]
        public string meaning { get; set; } = null!;

        [Required(ErrorMessage = "詞性不能為空")]
        [DisplayName("詞性")]
        [StringLength(10, ErrorMessage = "詞性長度不能超過10個字元")]
        public string parts_of_speech { get; set; } = null!;
        [DisplayName("例句")]
        public string Example { get; set; } = null!;

        [DisplayName("KK音標")]
        [StringLength(50, ErrorMessage = "KK音標長度不能超過50個字元")]
        public string KK { get; set; } = null!;

        [Required(ErrorMessage = "分類編號不能為空")]
        [DisplayName("分類編號")]
        public int categories_id { get; set; }

        [Required(ErrorMessage = "難度等級不能為空")]
        [Range(1,4, ErrorMessage = "難度等級只能介於1到4")]
        [DisplayName("難度等級")]
        public byte difficulty_level { get; set; }
    }
}
