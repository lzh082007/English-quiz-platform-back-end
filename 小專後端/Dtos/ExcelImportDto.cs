using System.ComponentModel;

namespace 小專後端.Dtos
{
    public class ExcelImportDto
    {
        [DisplayName("英文單字主體")]
        public string spelling { get; set; } = null!;
        [DisplayName("中文單字主體")]
        public string meaning { get; set; } = null!;
        [DisplayName("詞性")]
        public string parts_of_speech { get; set; } = null!;
        [DisplayName("例句")]
        public string Example { get; set; } = null!;
        [DisplayName("音標")] 
        public string KK { get; set; } = null!;
        [DisplayName("類別")]
        public int categories_id { get; set; }
        [DisplayName("難易度等級")]
        public byte difficulty_level { get; set; }
    }
}
