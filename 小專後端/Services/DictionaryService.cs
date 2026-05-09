using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using 小專後端.Dtos;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class DictionaryService 
    {
        private readonly IConfiguration _config;//這是讀取設定檔的

        private readonly englishContext _dbContext;//這是讀取資料庫用的
        public DictionaryService(IConfiguration config, englishContext dbContext)
        {//建構子注入IConfiguration和DbContext，這樣就可以在服務中使用它們來讀取設定和資料庫
            _config = config;
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {//從categories表格中查詢所有的類別，並且轉換成CategoryDto物件的集合，最後回傳這個集合
            return await _dbContext.categories
                .Select(c => new CategoryDto
                {
                    cid = c.cid,
                    categories = c.categories1
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<WordDto>> GetWordsAsync(int categoryId, byte difficultyLevel)
        {//從words表格中查詢指定類別ID和難度等級的單字，並且轉換成WordDto物件的集合，最後回傳這個集合
            return await _dbContext.words
                .Where(w => w.categories_id == categoryId && w.difficulty_level == difficultyLevel)
                .Select(w => new WordDto
                {
                    Wid = w.wid,
                    spelling = w.spelling,
                    meaning = w.meaning,
                    KK = w.KK,
                    parts_of_speech = FormatPartsOfSpeech(w.parts_of_speech),
                    Example = w.Example
                })
                .ToListAsync();
        }
        private static string FormatPartsOfSpeech(string pos)//把詞性縮寫轉換成完整的詞性名稱，並且格式化成小寫
        {
            if (string.IsNullOrEmpty(pos)) return "";

            return pos.ToLower().Trim() switch//使用switch表達式來根據詞性縮寫返回對應的完整詞性名稱，如果沒有匹配到就返回原始的pos值
            {
                "n" => "名詞(n)",
                "v" => "動詞(V)",
                "adj" => "形容詞(adj)",
                "adv" => "副詞(adv)",
                "v/n" => "動詞/名詞(v/n)",
                _ => pos
            };
        }
      
    }
}
