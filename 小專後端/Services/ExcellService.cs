using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Linq;
using 小專後端.Dtos;
using 小專後端.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace 小專後端.Services
{
    public class ExcellService
    {
        private readonly IConfiguration _config;//這是讀取設定檔的

        private readonly englishContext _dbContext;//這是讀取資料庫用的

        public ExcellService(IConfiguration config, englishContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }
        //管理匯入單字
        public async Task<List<string>> ProcessWordExcelAsync(IFormFile file)
        {
            var duplicateMessages = new List<string>();

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Execel_Temp");
            if (!Directory.Exists(directoryPath))//如果資料夾不存在就建立一個新的資料夾
            {
                Directory.CreateDirectory(directoryPath); //建立資料夾
            }

            var filePath = Path.Combine(directoryPath, $"{Guid.NewGuid()}.xlsx");//建立一個新的Excel檔案路徑，使用Guid來確保檔案名稱的唯一性

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);// 把上傳的Excel檔案複製到剛剛建立的檔案路徑中，這樣就可以在後續的程式中讀取這個Excel檔案
            }

            var parsedData = new List<(int Row, ExcelImportDto Dto)>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = package.Workbook.Worksheets[0];
                if (sheet.Dimension == null) return duplicateMessages;

                int rowCount = sheet.Dimension.End.Row;

                var categoryMap = new Dictionary<string, int>
                {//這裡是建立一個字典來對應Excel中的類別名稱和資料庫中的類別ID，這樣就可以在匯入的時候根據Excel中的類別名稱找到對應的類別ID
                     { "動物", 1 }, { "飲食", 2 }, { "交通", 3 },
                     { "生活用品", 4 }, { "教育", 5 }
                };

                for (int row = 2; row <= rowCount; row++)
                {
                    var spelling = sheet.Cells[row, 1].Text.Trim();
                    if (string.IsNullOrWhiteSpace(spelling)) continue;//如果英文單字欄位是空的或只有空白，就跳過這一列

                    var dto = new ExcelImportDto
                    {
                        spelling = spelling, 
                        meaning = sheet.Cells[row, 2].Text.Trim(),
                        parts_of_speech = sheet.Cells[row, 5].Text.Trim(),
                        KK = sheet.Cells[row, 6].Text.Trim(),
                        Example = sheet.Cells[row, 7].Text.Trim()
                    };
                    //這裡就是判斷分類裡面的中文對應ID
                    string categoryText = sheet.Cells[row, 3].Text?.Trim() ?? "";//這裡是從Excel的第三欄讀取類別名稱，並且去除前後的空白，如果讀取到的值是null就用空字串代替
                    if (categoryMap.ContainsKey(categoryText))
                    {
                        dto.categories_id = categoryMap[categoryText];
                    }
                    else if (int.TryParse(categoryText, out int cid) && cid >= 1 && cid <= 5)//如果Excel裡面的類別欄位不是中文名稱，而是直接填寫了類別ID，這裡就嘗試把它解析成整數，並且檢查是否在有效的範圍內，如果是的話就直接使用這個ID
                    {
                        dto.categories_id = cid;
                    }
                    else
                    {
                        dto.categories_id = 1;
                    }

                    if (byte.TryParse(sheet.Cells[row, 4].Text.Trim(), out byte diffLevel))//這裡是從Excel的第四欄讀取難度等級，並且嘗試把它解析成byte類型，如果解析成功就把它賦值給dto的difficulty_level屬性，否則就不賦值，保持默認值
                    {
                        dto.difficulty_level = diffLevel;
                    }

                    parsedData.Add((row, dto));
                }
            }

            if (parsedData.Any())
            {
                var excelSpellingsToLower = parsedData.Select(d => d.Dto.spelling.ToLower()).Distinct().ToList();//把資料都轉換成小寫，並替除掉重複選向(Distinct)

                var existingWords = await _dbContext.words
                    .Where(w => excelSpellingsToLower.Contains(w.spelling.ToLower()))
                    .Select(w => w.spelling.ToLower())
                    .ToListAsync();

                var newEntities = new List<words>();

                var pendingToAdd = new HashSet<string>();

                foreach (var item in parsedData)
                {
                    var checkSpelling = item.Dto.spelling.ToLower();

                    if (existingWords.Contains(checkSpelling))
                    {
                        duplicateMessages.Add($"第{item.Row}列 單字{item.Dto.spelling}已存在");
                    }
                    else if (pendingToAdd.Contains(checkSpelling))
                    {
                        duplicateMessages.Add($"第{item.Row}列 單字{item.Dto.spelling}在表單內重複");
                    }
                    else
                    {
                        pendingToAdd.Add(checkSpelling);
                        newEntities.Add(new words
                        {
                            spelling = item.Dto.spelling,
                            meaning = item.Dto.meaning,
                            parts_of_speech = item.Dto.parts_of_speech,
                            KK = item.Dto.KK,
                            categories_id = item.Dto.categories_id,
                            difficulty_level = (byte)item.Dto.difficulty_level,
                            Example = item.Dto.Example
                        });
                    }
                }

                if (newEntities.Any())
                {
                    _dbContext.words.AddRange(newEntities);
                    await _dbContext.SaveChangesAsync();
                }
            }
            return duplicateMessages;
        }
        //管理匯出單字
        public async Task<byte[]> ExportWordsToExcelAsync()//從資料庫中查詢所有的單字，然後使用EPPlus套件來建立一個Excel檔案，並且把查詢到的單字資料填入到Excel檔案中，最後把Excel檔案轉換成byte陣列回傳
        {
            var words = await _dbContext.words.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Words Data");

                // 設定表頭
                sheet.Cells[1, 1].Value = "英文單字";
                sheet.Cells[1, 2].Value = "中文翻譯";
                sheet.Cells[1, 3].Value = "類別ID";
                sheet.Cells[1, 4].Value = "難易度";
                sheet.Cells[1, 5].Value = "詞性";
                sheet.Cells[1, 6].Value = "音標";
                sheet.Cells[1, 7].Value = "例句";

                // 填入資料
                int row = 2;
                foreach (var word in words)
                {
                    sheet.Cells[row, 1].Value = word.spelling;
                    sheet.Cells[row, 2].Value = word.meaning;
                    sheet.Cells[row, 3].Value = word.categories_id;
                    sheet.Cells[row, 4].Value = word.difficulty_level;
                    sheet.Cells[row, 5].Value = word.parts_of_speech;
                    sheet.Cells[row, 6].Value = word.KK;
                    sheet.Cells[row, 7].Value = word.Example;
                    row++;
                }

                //自動調整所有欄寬
                sheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        //管理單字模板
        public byte[] GetExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("單字匯入模板");
                sheet.Cells[1, 1].Value = "英文單字";
                sheet.Cells[1, 2].Value = "中文翻譯";
                sheet.Cells[1, 3].Value = "類別ID";
                sheet.Cells[1, 4].Value = "難易度(1-4)";
                sheet.Cells[1, 5].Value = "詞性";
                sheet.Cells[1, 6].Value = "音標";
                sheet.Cells[1, 7].Value = "例句";

                sheet.Cells[2, 1].Value = "apple";
                sheet.Cells[2, 2].Value = "蘋果";
                sheet.Cells[2, 3].Value = 1;        
                sheet.Cells[2, 4].Value = 1;       
                sheet.Cells[2, 5].Value = "n";      
                sheet.Cells[2, 6].Value = "/ˈæp.əl/";
                sheet.Cells[2, 7].Value = "I ate an apple yesterday.";
                using (var range = sheet.Cells[1, 1, 1, 7])//加粗線
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
                sheet.Cells.AutoFitColumns();//自動調整蘭寬
                return package.GetAsByteArray();
            }
        }
        //管理題目
        public async Task<List<string>> QuestionExcelImpot(IFormFile file)
        {//這個方法是用來處理上傳的Excel檔案，從Excel中讀取題目資料，然後把這些資料匯入到資料庫中，如果在匯入的過程中發現有重複的題目或是找不到對應的單字，就把這些錯誤訊息收集起來，最後回傳這些錯誤訊息的列表
            var errorMessages = new List<string>(); 

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "question_Temp");//這裡是建立一個臨時資料夾來存放上傳的Excel檔案，這樣就不會把上傳的檔案直接放在專案的根目錄下，保持專案的整潔
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, $"{Guid.NewGuid()}.xlsx");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var parsedData = new List<(int Row, QuestTempDto Dto, string WordSpelling)>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = package.Workbook.Worksheets[0];//抓execl第一頁
                if (sheet.Dimension == null) return errorMessages;

                int rowCount = sheet.Dimension.End.Row;
                var typeMap = new Dictionary<string, int>
                {
                      { "克漏字", 1 },
                      { "聽力", 2 },
                      { "閱讀測驗", 3 }
                };

                for (int row = 2; row <= rowCount; row++)
                {
                    var content = sheet.Cells[row, 3].Text.Trim();
                    if (string.IsNullOrWhiteSpace(content)) continue;

                    var dto = new QuestTempDto
                    {
                        question_content = content
                    };

                    string wordSpelling = sheet.Cells[row, 1].Text.Trim();

                    string typeText = sheet.Cells[row, 2].Text?.Trim() ?? "";
                    if (typeMap.ContainsKey(typeText))
                    {
                        dto.question_type_id = typeMap[typeText];
                    }
                    else if (int.TryParse(typeText, out int tid) && tid >= 1 && tid <= 3)
                    {
                        dto.question_type_id = tid;
                    }
                    else
                    {
                        dto.question_type_id = 1;
                    }

                    parsedData.Add((row, dto, wordSpelling));
                }
            }

            if (parsedData.Any())//如果有資料
            {
                var excelContents = parsedData.Select(d => d.Dto.question_content).Distinct().ToList();//這裡抓出question_content所有題目內容
                var existingQuestions = await _dbContext.questions
                    .Where(q => excelContents.Contains(q.question_content))
                    .Select(q => q.question_content)
                    .ToListAsync();

                var excelWordsToLower = parsedData.Select(d => d.WordSpelling.ToLower()).Distinct().ToList();//這裡抓出Excel裡面所有的單字，並且轉換成小寫，這樣就可以在後續的查詢中忽略大小寫的差異
                var dbWords = await _dbContext.words
                    .Where(w => excelWordsToLower.Contains(w.spelling.ToLower()))
                    .Select(w => new { w.wid, w.spelling })
                    .ToListAsync();
                var wordDict = dbWords.ToDictionary(w => w.spelling.ToLower(), w => w.wid);

                var newEntities = new List<questions>();
                var pendingToAdd = new HashSet<string>();

                foreach (var item in parsedData)//這裡是逐行檢查Excel裡面的資料，首先檢查單字欄位，如果單字是空的或只有空白，就記錄一個錯誤訊息並且跳過這一列；然後檢查這個單字在資料庫裡面是否存在，如果不存在就記錄一個錯誤訊息並且跳過這一列；如果單字存在，就把它對應的ID賦值給DTO的word_id屬性；接著檢查題目內容是否已經存在於資料庫裡面，如果存在就記錄一個錯誤訊息；再檢查這個題目內容在Excel表單內是否重複，如果重複也記錄一個錯誤訊息；如果都沒有問題，就把這個題目加入到待新增的清單中，最後一次性地把所有新的題目加入到資料庫中
                {
                    var checkContent = item.Dto.question_content;
                    var checkWord = item.WordSpelling.ToLower();

                    if (string.IsNullOrWhiteSpace(checkWord))
                    {
                        errorMessages.Add($"第{item.Row}列 尚未填寫對應單字");
                        continue; 
                    }
                    else if (!wordDict.ContainsKey(checkWord))
                    {
                        errorMessages.Add($"第{item.Row}列 找不到單字「{item.WordSpelling}」，請先匯入該單字");
                        continue; 
                    }

                    item.Dto.word_id = wordDict[checkWord];
                    if (existingQuestions.Contains(checkContent))
                    {
                        errorMessages.Add($"第{item.Row}列 題目已存在");
                    }
                    else if (pendingToAdd.Contains(checkContent))
                    {
                        errorMessages.Add($"第{item.Row}列 題目在表單內重複");
                    }
                    else
                    {
                        pendingToAdd.Add(checkContent);
                        newEntities.Add(new questions
                        {
                            word_id = item.Dto.word_id, 
                            question_type_id = item.Dto.question_type_id,
                            question_content = item.Dto.question_content,
                            question_correct = 0,
                            question_error = 0
                        });
                    }
                }

                if (newEntities.Any())
                {
                    _dbContext.questions.AddRange(newEntities);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return errorMessages;
        }
        public async Task<byte[]> OputQuestions()//這個方法是從資料庫中查詢所有的題目，然後把這些題目資料轉換成Excel檔案，最後把Excel檔案轉換成byte陣列回傳
        {
            //word_ID轉成文字
            var questionList = await (from q in _dbContext.questions
            join w in _dbContext.words on q.word_id equals w.wid
            select new
            {
                WordSpelling = w.spelling,
                TypeId = q.question_type_id,
                Content = q.question_content,
                CorrectCount = q.question_correct, 
                ErrorCount = q.question_error      
            }).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Questions Data");

                sheet.Cells[1, 1].Value = "word(英文單字)";
                sheet.Cells[1, 2].Value = "question_type_id(題型ID)";
                sheet.Cells[1, 3].Value = "question_content";
                sheet.Cells[1, 4].Value = "question_correct(答對次數)"; 
                sheet.Cells[1, 5].Value = "question_error(答錯次數)"; 

                using (var range = sheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // 填入資料
                int row = 2;
                foreach (var q in questionList)
                {
                    sheet.Cells[row, 1].Value = q.WordSpelling;

                   
                    string typeName = q.TypeId == 1 ? "克漏字" : (q.TypeId == 2 ? "聽力" : (q.TypeId == 3 ? "閱讀測驗" : q.TypeId.ToString()));
                    sheet.Cells[row, 2].Value = typeName;

                    sheet.Cells[row, 3].Value = q.Content;
                    sheet.Cells[row, 4].Value = q.CorrectCount;
                    sheet.Cells[row, 5].Value = q.ErrorCount;   

                    row++;
                }

                // 自動調整所有欄寬
                sheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        public byte[] GetQuestionExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("題目匯入模板");

                // 設定表頭
                sheet.Cells[1, 1].Value = "word(英文單字)";
                sheet.Cells[1, 2].Value = "question_type_id(題型ID)";
                sheet.Cells[1, 3].Value = "question_content";

                sheet.Cells[2, 1].Value = "bird";
                sheet.Cells[2, 2].Value = "克漏字";
                sheet.Cells[2, 3].Value = "A ____ landed on the window this morning.";

                // 設定表頭樣式
                using (var range = sheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;//設定填充樣式為實心
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue); //設定背景顏色
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White); //設定字體顏色
                }

                // 自動調整欄寬
                sheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }

}

