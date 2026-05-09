using Microsoft.EntityFrameworkCore;
using 小專後端.Dtos;
using 小專後端.Dtos.quiz;
using 小專後端.Dtos.Quiz;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class QuizService
    {
        private readonly englishContext _dbContext;

        public QuizService(englishContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<QuizQuestionDto> GetQuizQuestions(int level, int category)
        {
            List<QuizQuestionDto> quizQuestions = new List<QuizQuestionDto>();
            Random random = new Random();
            try
            {
                List<questions> questiondata = _dbContext.questions
                                               .Include(q => q.word)//Include方法用來指定在查詢questions表格時，同時載入關聯的word資料，這樣就可以在後續的程式碼中直接使用question.word來存取相關的單字資訊，而不需要再額外查詢一次資料庫
                                               .Where(q => q.word.difficulty_level == level && q.word.categories_id == category)
                                               .OrderBy(q => Guid.NewGuid())
                                               .Take(10) //Take意思是從符合條件
                                               .ToList();//從資料庫中查詢符合難度等級和類別的題目，並且隨機打亂順序，最後取前10題
                foreach (var question in questiondata)
                {
                    List<string> erroroptions = (from w in _dbContext.words
                                                 where w.categories_id != category
                                                 orderby Guid.NewGuid()
                                                 select w.spelling)
                                             .Take(3)
                                             .ToList();
                    int randomIndex = random.Next(questiondata.Count);
                    string Answer = question.word.spelling;

                    List<string> options = new List<string>//建立一個選項列表，包含三個錯誤選項和一個正確答案
                    {
                        erroroptions[0],
                        erroroptions[1],
                        erroroptions[2],
                        Answer
                    };

                    List<string> randomOptions = options.OrderBy(x => random.Next()).ToList();

                    quizQuestions.Add(new QuizQuestionDto
                    {
                        QuestionId = question.qid,
                        QuestionContent = questiondata[randomIndex].question_content,
                        OptionA = randomOptions[0],
                        OptionB = randomOptions[1],
                        OptionC = randomOptions[2],
                        OptionD = randomOptions[3],
                        Answer = Answer
                    });
                }
                return quizQuestions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<QuizQuestionDto> GetCustomListQuizQuestions(int listId)
        {//這個方法是從指定的自訂清單中隨機抽取10題，並且為每題生成三個錯誤選項，最後回傳一個包含題目內容、選項和答案的QuizQuestionDto列表
            List<QuizQuestionDto> quizQuestions = new List<QuizQuestionDto>();
            Random random = new Random();

            try
            {
                var questionData = (from li in _dbContext.list_items
                                    join q in _dbContext.questions on li.word_id equals q.word_id
                                    join w in _dbContext.words on li.word_id equals w.wid
                                    where li.list_id == listId
                                    orderby Guid.NewGuid() 
                                    select new { Question = q, Word = w })
                                   .Take(10) 
                                   .ToList();

                foreach (var item in questionData)
                {
                    string correctAnswer = item.Word.spelling;

                    List<string> errorOptions = _dbContext.words
                        .Where(w => w.spelling != correctAnswer)
                        .OrderBy(w => Guid.NewGuid())
                        .Select(w => w.spelling)
                        .Take(3)
                        .ToList();


                    List<string> options = new List<string>
                    {
                       errorOptions[0],
                       errorOptions[1],
                       errorOptions[2],
                       correctAnswer
                    };

                    List<string> randomOptions = options.OrderBy(x => random.Next()).ToList();

                    quizQuestions.Add(new QuizQuestionDto
                    {
                        QuestionId= item.Question.qid,
                        QuestionContent = item.Question.question_content,
                        OptionA = randomOptions[0],
                        OptionB = randomOptions[1],
                        OptionC = randomOptions[2],
                        OptionD = randomOptions[3],
                        Answer = correctAnswer
                    });
                }

                return quizQuestions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> SubmitQuizResultAsync(int userId, SubmitQuizDto resultData)
        {//這個方法是用來處理使用者提交的測驗結果
            try
            {
                var errorWordStrings = new HashSet<string>();
                foreach (var detail in resultData.Details)
                {
                    if (!string.IsNullOrWhiteSpace(detail.Error_A)) errorWordStrings.Add(detail.Error_A);
                    if (!string.IsNullOrWhiteSpace(detail.Error_B)) errorWordStrings.Add(detail.Error_B);
                    if (!string.IsNullOrWhiteSpace(detail.Error_C)) errorWordStrings.Add(detail.Error_C);
                }//建立一個字串到ID的對照字典，這樣後續在儲存錯誤選項時就可以直接使用這個字典來找到對應的單字ID，而不需要再額外查詢一次資料庫

                var wordIdDict = await _dbContext.words
                     .Where(w => errorWordStrings.Contains(w.spelling))
                     .ToDictionaryAsync(w => w.spelling, w => w.wid);

                // 新增測驗總表(quiz_records)
                var record = new quiz_records
                {
                    user_id = userId,
                    source = resultData.Source,
                    list_id = resultData.ListId,
                    quiz_at = DateTime.Now,
                    total_time_spent = TimeOnly.FromTimeSpan(resultData.TotalTimeSpent),
                    correct_count = resultData.CorrectCount,
                    wrong_count = resultData.WrongCount
                };

                _dbContext.quiz_records.Add(record);
                await _dbContext.SaveChangesAsync(); 

                foreach (var detail in resultData.Details)
                {
                    var log = new quiz_detail_logs
                    {
                        quiz_record_id = record.id,
                        question_id = detail.QuestionId,
                        is_wrong = detail.IsWrong,
                        user_answer = detail.UserAnswer ?? "",
                    };
                    _dbContext.quiz_detail_logs.Add(log);
                }

                await _dbContext.SaveChangesAsync();

                foreach (var detail in resultData.Details)
                {
                    if (wordIdDict.TryGetValue(detail.Error_A ?? "", out int errorIdA))//TryGetValue方法用來嘗試從字典中取得指定鍵的值，如果找到就回傳true並且把值存到errorIdA變數中，這樣就可以避免在儲存錯誤選項時因為找不到對應的單字而發生例外
                    {
                        _dbContext.quiz_error_option.Add(new quiz_error_option { quiz_record_id = record.id, question_id = detail.QuestionId, option_word_id = errorIdA });
                    }
                    if (wordIdDict.TryGetValue(detail.Error_B ?? "", out int errorIdB))
                    {
                        _dbContext.quiz_error_option.Add(new quiz_error_option { quiz_record_id = record.id, question_id = detail.QuestionId, option_word_id = errorIdB });
                    }
                    if (wordIdDict.TryGetValue(detail.Error_C ?? "", out int errorIdC))
                    {
                        _dbContext.quiz_error_option.Add(new quiz_error_option { quiz_record_id = record.id, question_id = detail.QuestionId, option_word_id = errorIdC });
                    }
                }

                await _dbContext.SaveChangesAsync();

                return record.id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<QuizHistoryDto>> GetQuizHistoryAsync(int userId)
        {//這個方法是用來取得使用者的測驗歷史紀錄，包含每次測驗的來源、日期、花費時間、正確和錯誤的題數等資訊
            var query = from r in _dbContext.quiz_records
                        where r.user_id == userId
 
                        join l in _dbContext.personal_lists on r.list_id equals l.lid into listGroup
                        from lg in listGroup.DefaultIfEmpty() 
                        orderby r.quiz_at descending
                        select new
                        {
                            Record = r,
                            ListName = lg != null ? lg.title : null 
                        };

            var records = await query.ToListAsync();

            var historyList = records.Select(x => new QuizHistoryDto
            {
                RecordId = x.Record.id,

                SourceName = x.Record.source == 1 ? "字典分類" :
                            (x.Record.source == 2 ? $"自訂清單 ({x.ListName ?? "已刪除清單"})" : "其他"),

                QuizDate = x.Record.quiz_at?.ToString("yyyy-MM-dd HH:mm") ?? "",

                TotalTime = x.Record.total_time_spent.ToString(@"mm\:ss") ?? "00:00",

                CorrectCount = x.Record.correct_count,
                WrongCount = x.Record.wrong_count
            }).ToList();

            return historyList;
        }
        public async Task<List<QuizResponseDto>> GetQuizRecordDetailsAsync(int recordId)
        {//這個方法是用來取得指定測驗紀錄的詳細資訊，包含每題的題目內容、使用者答案、正確答案、是否答錯以及錯誤選項等資訊
            try
            {
                //三張表 JOIN 起來
                var details = await (from log in _dbContext.quiz_detail_logs
                                     join q in _dbContext.questions on log.question_id equals q.qid 
                                     join w in _dbContext.words on q.word_id equals w.wid           
                                     where log.quiz_record_id == recordId
                                     select new QuizResponseDto
                                     {
                                         QuestionId=q.qid,
                                         QuestionContent = q.question_content,
                                         UserAnswer = log.user_answer,
                                         CorrectAnswer = w.spelling,
                                         IsWrong = log.is_wrong,
                                         ErrorOptions = (from opt in _dbContext.quiz_error_option
                                                         join optWord in _dbContext.words on opt.option_word_id equals optWord.wid
                                                         where opt.quiz_record_id == recordId && opt.question_id == q.qid
                                                         select optWord.spelling).ToList()
                                     }).ToListAsync();
                return details;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<QuestionListDto>> GetTopicAsync(byte? difficultyLevel, int? categoryId)
        {
            var query = _dbContext.questions
                .Include(q => q.word)
                .ThenInclude(w => w.categories)
                .Where(q => q.IsDeleted == false)//這裡主抓沒有被刪除的
                .AsQueryable();

            if (difficultyLevel.HasValue)
            {
                query = query.Where(q => q.word.difficulty_level == difficultyLevel.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(q => q.word.categories_id == categoryId.Value);
            }

            var result = await query.Select(q => new QuestionListDto
            {
                qid = q.qid,
                word_id = q.word_id,
                question_content = q.question_content,
                question_type_id = q.question_type_id,
                spelling = q.word.spelling,
                difficulty_level = q.word.difficulty_level,
                CategoryName = q.word.categories != null ? q.word.categories.categories1 : "無分類"
            }).ToListAsync();

            return result;
        }
        // 更新題目
        public async Task<bool> UpdateQuestionAsync(int qid, QuestionUpdateDto dto)
        {
            // 去資料庫找這筆題目，並把關聯的單字一起抓出來
            var questionToUpdate = await _dbContext.questions
                .Include(q => q.word)
                .FirstOrDefaultAsync(q => q.qid == qid);

            if (questionToUpdate == null) return false;

            questionToUpdate.question_content = dto.content;

            if (questionToUpdate.word != null)
            {
                questionToUpdate.word.spelling = dto.answer;
                questionToUpdate.word.categories_id = dto.categories_id;
                questionToUpdate.word.difficulty_level = dto.difficulty_level;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        //刪除題目
        public async Task<bool> DeleteQuestionAsync(int qid)
        {
            var questionToDelete = await _dbContext.questions.FindAsync(qid);

            if (questionToDelete == null) return false;

            questionToDelete.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
