using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using 小專後端.Dtos;
using 小專後端.Dtos.quiz;
using 小專後端.Dtos.Quiz;
using 小專後端.Services;
namespace 小專後端.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly QuizService _quizService;
        public QuizController(QuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("questions")]//根據難度等級和類別來取得測驗題目
        public IActionResult GetQuizQuestions([FromBody] QuizFilterDto dto)
        {
            try
            {
                var questions = _quizService.GetQuizQuestions(dto.Level, dto.Category);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("custom-list/{listId}")]//根據自訂清單ID來取得測驗題目
        public IActionResult GetCustomListQuiz(int listId)
        {
            try
            {
                var questions = _quizService.GetCustomListQuizQuestions(listId);

                if (questions.Count == 0)
                {
                    return NotFound(new { message = "這個清單裡面沒有題目" });
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("submit")]//提交測驗成績
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto request)
        {
            try
            {
                //從Token抓取使用者 ID
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new { message = "Token無效" });
                }

                int record_id = await _quizService.SubmitQuizResultAsync(userId, request);

                return Ok(new { message = "測驗成績成功儲存", record_id = record_id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("history")]//取得使用者的測驗歷史紀錄
        public async Task<IActionResult> GetQuizHistory()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new { message = "Token無效或找不到使用者ID" });
                }

                var history = await _quizService.GetQuizHistoryAsync(userId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("history/{recordId}")]//取得單筆測驗紀錄的詳細資訊
        public async Task<IActionResult> GetQuizDetail(int recordId)
        {
            try
            {
                var details = await _quizService.GetQuizRecordDetailsAsync(recordId);

                if (details == null || details.Count == 0)
                {
                    return NotFound(new { message = "找不到這筆測驗的明細資料" });
                }

                return Ok(details);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("topic")]//根據難度等級和類別來取得測驗主題
        public async Task<IActionResult> GetTopic([FromQuery] byte? difficultyLevel, [FromQuery] int? categoryId)
        {
            try
            {
                var topic = await _quizService.GetTopicAsync(difficultyLevel, categoryId);
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"單字列表錯誤:{ex.Message}");
            }
        }
        [HttpPut("{qid}")]//更新題目
        public async Task<IActionResult> UpdateQuestion(int qid, [FromBody] QuestionUpdateDto dto)
        {
            if (qid != dto.qid)
            {
                return BadRequest(new { message = "資料ID不符" });
            }

            try
            {
                //呼叫Service更新
                var isUpdated = await _quizService.UpdateQuestionAsync(qid, dto);

                if (isUpdated)
                {
                    return Ok(new { message = $"題目ID:{qid}更新成功" });
                }
                else
                {
                    return NotFound(new { message = "找不到該題目，更新失敗" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新錯誤: {ex.Message}");
            }
        }
        [HttpDelete("{qid}")]//刪除題目
        public async Task<IActionResult> DeleteWord(int qid)
        {
            try
            {
                var isDeleted = await _quizService.DeleteQuestionAsync(qid);

                if (isDeleted)
                {
                    return Ok(new { message = $"單字ID:{qid}刪除成功" });
                }
                else
                {
                    return NotFound(new { message = "找不到該單字" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"刪除錯誤:{ex.Message}");
            }
        }
    }
}
