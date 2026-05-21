using Microsoft.EntityFrameworkCore;
using System.Linq;
using 小專後端.Dtos;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class WordService
    {
        private readonly IConfiguration _config;//這是讀取設定檔的

        private readonly englishContext _dbContext;//這是讀取資料庫用的
        public WordService(IConfiguration config, englishContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }
        public async Task<bool> UpdateSingleWordAsync(int wid, WordUpdateDto dto)
        {
            //去資料庫找這筆單字
            var wordToUpdate = await _dbContext.words.FindAsync(wid);

            
            if (wordToUpdate == null) return false;

            //把新資料覆蓋上去
            wordToUpdate.spelling = dto.spelling;
            wordToUpdate.meaning = dto.meaning;
            wordToUpdate.parts_of_speech = dto.parts_of_speech;
            wordToUpdate.KK = dto.KK;
            wordToUpdate.categories_id = dto.categories_id;
            wordToUpdate.difficulty_level = dto.difficulty_level;
            wordToUpdate.Example = dto.Example;

            //存檔
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteSingleWordAsync(int wid)
        {
            var wordToDelete = await _dbContext.words.FindAsync(wid);

            if (wordToDelete == null) return false;

            _dbContext.words.Remove(wordToDelete);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<WordListDto>> GetWordsAsync(byte? difficultyLevel, int? categoryId)
        {//從資料庫中查詢符合指定難度等級和類別ID的單字，並且轉換成WordListDto物件的集合，最後回傳這個集合

            var query = _dbContext.words
                .Include(w => w.categories)
                .AsQueryable();

            if (difficultyLevel.HasValue)
            {
                query = query.Where(w => w.difficulty_level == difficultyLevel.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(w => w.categories_id == categoryId.Value);
            }

            var result = await query.Select(w => new WordListDto
            {
                wid = w.wid,
                spelling = w.spelling,
                meaning = w.meaning ??"",
                parts_of_speech = w.parts_of_speech ??"",
                KK = w.KK ?? "",
                difficulty_level = w.difficulty_level,
                categories_id = w.categories_id,
                Example = w.Example ?? "",
                CategoryName = w.categories != null ? w.categories.categories1 :"無分類"
            }).ToListAsync();

            return result;
        }

       
      
    }
}
