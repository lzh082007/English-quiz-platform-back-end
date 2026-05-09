using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using 小專後端.Dtos;
using 小專後端.Dtos.list;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class CustomListService
    {
        private readonly englishContext _dbContext;

        public CustomListService(englishContext dbContext)//建構子注入DbContext
        {
            _dbContext = dbContext;
        }

        // 把單字加入清單
        public async Task<bool> AddWordToListAsync(int listId, int wordId)//回傳是否成功加入（如果已經存在就不加入）
        {
            var isExist = await _dbContext.list_items
                .AnyAsync(x => x.list_id == listId && x.word_id == wordId);//檢查是否已經存在AnyAsync會回傳true或false

            if (isExist) return false;

            var newItem = new list_items//建立新的list_items物件
            {
                list_id = listId,
                word_id = wordId,
                created_at = DateTime.Now 
            };

            _dbContext.list_items.Add(newItem);//加入到DbContext的list_items集合中
            await _dbContext.SaveChangesAsync();//儲存變更到資料庫
            return true;
        }

        //取得使用者的所有清單
        public async Task<IEnumerable<CustomListDto>> GetUserListsAsync(int userId)//回傳使用者的所有清單，包含清單內的單字數量和最後更新時間
        {
            return await _dbContext.personal_lists//從personal_lists表格中查詢
                .Where(p => p.user_id == userId)
                .OrderByDescending(p => p.created_at)
                .Select(p => new CustomListDto
                {
                    ListId = p.lid,
                    Title = p.title,
                    Description = p.description,
                    CreatedAt = p.created_at,
                    WordCount = _dbContext.list_items.Count(li => li.list_id == p.lid),
                    LastUpdated = _dbContext.list_items
                                  .Where(li => li.list_id == p.lid)
                                  .Max(li => (DateTime?)li.created_at) ?? p.created_at
                })
                .ToListAsync();
        }

        //取得清單內的單字
        public async Task<IEnumerable<WordDto>> GetListWordsAsync(int listId, byte? difficultyLevel = null)
        {//從list_items表格中查詢指定清單ID的單字，並且可以根據難度等級過濾
            var query = from li in _dbContext.list_items
                        join w in _dbContext.words on li.word_id equals w.wid
                        where li.list_id == listId
                        select w;

            if (difficultyLevel.HasValue && difficultyLevel.Value > 0)
            {
                query = query.Where(w => w.difficulty_level == difficultyLevel.Value);
            }

            var words = await query.Select(w => new WordDto
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
        //新增清單
        public async Task<int> CreateListAsync(int userId, string title, string description)
        {//建立新的personal_lists物件，並且加入到DbContext的personal_lists集合中，最後儲存變更到資料庫，回傳新建立的清單ID
            var newList = new personal_lists
            {
                user_id = userId,
                title = title,
                description = description,
                created_at = DateTime.Now
            };

            _dbContext.personal_lists.Add(newList);
            await _dbContext.SaveChangesAsync();//儲存變更到資料庫，這時候newList物件的lid屬性就會被資料庫自動生成並且更新，所以可以直接回傳newList.lid

            //回傳新建立的清單ID
            return newList.lid;
        }
        //更改清單
        public async Task<bool> UpdateListAsync(int listId, int userId, string newTitle, string newDescription)
        {//從personal_lists表格中查詢指定清單ID和使用者ID的清單，如果找不到就回傳false，否則更新清單的標題和描述，最後儲存變更到資料庫並且回傳true
            //加入判斷只能自己更改
            var list = await _dbContext.personal_lists
                .FirstOrDefaultAsync(l => l.lid == listId && l.user_id == userId);

            if (list == null) return false;

            list.title = newTitle;
            list.description = newDescription;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        //刪除清單
        public async Task<bool> DeleteListAsync(int listId, int userId)
        {//從personal_lists表格中查詢指定清單ID和使用者ID的清單，如果找不到就回傳false，否則先刪除清單內的所有單字，再刪除清單本身，最後儲存變更到資料庫並且回傳true
            var list = await _dbContext.personal_lists
                .FirstOrDefaultAsync(l => l.lid == listId && l.user_id == userId);

            if (list == null) return false;
            var itemsToDelete = _dbContext.list_items.Where(li => li.list_id == listId);
            _dbContext.list_items.RemoveRange(itemsToDelete);

            _dbContext.personal_lists.Remove(list);//刪除清單本身
            await _dbContext.SaveChangesAsync();
            return true;
        }
        //從清單裡面移除特定的單字
        public async Task<bool> RemoveWordFromListAsync(int listId, int wordId)//從list_items表格中查詢指定清單ID和單字ID的項目，如果找不到就回傳false，否則刪除該項目，最後儲存變更到資料庫並且回傳true
        {
            var item = await _dbContext.list_items
                .FirstOrDefaultAsync(li => li.list_id == listId && li.word_id == wordId);

            if (item == null) return false;

            _dbContext.list_items.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}