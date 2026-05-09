using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using 小專後端.Dtos;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class PractiseService
    {
        private readonly IConfiguration _config;//這是讀取設定檔的
        private readonly englishContext _dbContext;

        public PractiseService(englishContext dbContext)
        {
            _dbContext = dbContext;
        }

        //隨機抽取練習單字20題
        public async Task<IEnumerable<WordDto>> GetCustomListAsync(int listId, int count = 20)
        {
            var query = from li in _dbContext.list_items
                        join w in _dbContext.words on li.word_id equals w.wid
                        where li.list_id == listId
                        // 使用Guid.NewGuid()讓資料庫幫做隨機打亂
                        orderby Guid.NewGuid()
                        select w;

            //就是只抓前20個
            var words = await query.Take(count).Select(w => new WordDto
            {
                Wid = w.wid,
                spelling = w.spelling,
                meaning = w.meaning,
                KK = w.KK,
                parts_of_speech = FormatPartsOfSpeech(w.parts_of_speech),
                Example = w.Example
            }).ToListAsync();

            return words;
        }
        private static string FormatPartsOfSpeech(string pos)//把詞性縮寫轉換成完整的詞性名稱，並且格式化成小寫
        {
            if (string.IsNullOrEmpty(pos)) return "";
            return pos.ToLower().Trim() switch
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