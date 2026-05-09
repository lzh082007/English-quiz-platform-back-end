using Microsoft.EntityFrameworkCore;
using 小專後端.Dtos.ticket;
using 小專後端.Models;

namespace 小專後端.Services
{
    public class ProblemService
    {
        private readonly IConfiguration _englishContext;
        private readonly englishContext _dbContext;

        public ProblemService(IConfiguration configuration, englishContext dbContext)
        {
            _englishContext = configuration;
            _dbContext = dbContext;
        }

        public problem_list? CreateProblemTicket(ProblemDto dto)
        {//驗證傳入資料並將新問題單存入資料庫；若驗證失敗或發生例外則回傳 null。
            if (dto.reporter_id <= 0 || string.IsNullOrWhiteSpace(dto.description))
            {
                return null;
            }

            try
            {
                var newProblem = new problem_list
                {
                    reporter_id = dto.reporter_id,
                    question_id = (dto.question_id == null || dto.question_id <= 0) ? null : dto.question_id,
                    description = dto.description,
                    problem_type = dto.problem_type.ToString(),
                    status = false,
                    created_at = DateTime.Now 
                };

                _dbContext.problem_list.Add(newProblem);
                _dbContext.SaveChanges();//儲存到資料庫
                return newProblem;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }
        public async Task<List<AdminProblemListDto>> GetAllReportsAsync()
        {
            //將problem_list與Users進行JOIN，抓取Email
            return await _dbContext.problem_list
                .Join(_dbContext.users,
                    p => p.reporter_id,
                    u => u.uid,
                    (p, u) => new AdminProblemListDto
                    {
                        Pid = p.pid,
                        Email = u.email,
                        Description = p.description,
                        QuestionType = p.problem_type,
                        Status = p.status,
                        CreatedAt = p.created_at
                    })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        //取得問題單明細
        public async Task<AdminProblemDetailedDto> GetReportDetailAsync(int pid)
        {
            //先抓出問題單基本資料跟回報者信箱
            var report = await _dbContext.problem_list
                .Where(p => p.pid == pid)
                .Join(_dbContext.users,
                    p => p.reporter_id,
                    u => u.uid,
                    (p, u) => new { p, u.email })
                .FirstOrDefaultAsync();

            if (report == null) return null;

            // 組裝基本明細資料
            var detailDto = new AdminProblemDetailedDto
            {
                Pid = report.p.pid,
                Email = report.email,
                Description = report.p.description,
                QuestionType = report.p.problem_type,
                Status = report.p.status,
                CreatedAt = report.p.created_at
            };

            //如果是題目錯誤而且真的有綁定題目ID，再去資料庫撈題目
            if (report.p.problem_type == "題目錯誤" && report.p.question_id.HasValue)
            {
                detailDto.QuestionInfo = await _dbContext.questions
                    .Where(q => q.qid == report.p.question_id.Value)
                    .Select(q => new AdminProblemQustion
                    {
                        Qid = q.qid,
                        QuestionContent = q.question_content,
                        QuestionCorrect = q.question_correct,
                        QuestionError = q.question_error
                    })
                    .FirstOrDefaultAsync();
            }

            return detailDto;
        }

        //更新處理狀態
        public async Task<bool> UpdateStatusAsync(int pid, bool status)
        {
            // 先用主鍵找這筆問題單
            var report = await _dbContext.problem_list.FindAsync(pid);
            if (report == null) return false;

            // 更新狀態並存檔
            report.status = status;
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<List<AdminProblemListDto>> GetAllReportsAsync(string? questionType = null, string? keyword = null)
        {//這裡資料庫查詢問題單並且可以根據問題類型和關鍵字進行篩選
            var query = _dbContext.problem_list
                .Join(_dbContext.users,
                    p => p.reporter_id,
                    u => u.uid,
                    (p, u) => new AdminProblemListDto
                    {
                        Pid = p.pid,
                        Email = u.email,
                        Description = p.description,
                        QuestionType = p.problem_type,
                        Status = p.status,
                        CreatedAt = p.created_at
                    })
                .AsQueryable();

            //如果有傳類別，就篩選類別
            if (!string.IsNullOrEmpty(questionType))
            {
                query = query.Where(x => x.QuestionType == questionType);
            }

            // 如果有傳關鍵字，就模糊搜尋
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Email.Contains(keyword) || x.Description.Contains(keyword));
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }


    }
}