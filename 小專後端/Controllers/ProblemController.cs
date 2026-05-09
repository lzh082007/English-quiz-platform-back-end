using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using 小專後端.Dtos.ticket;
using 小專後端.Models;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProblemController : Controller
    {
        private readonly ProblemService _problemService;
        public ProblemController(ProblemService problemService)
        {
            _problemService = problemService;
        }
        [HttpPost("report")]//使用者回報問題
        public IActionResult ReportProblem([FromBody] ProblemDto dto)
        {
            var isSuccess = _problemService.CreateProblemTicket(dto);

            if (isSuccess != null)
            {
                return Ok(new { success = true, message = "問題回報成功！"});
            }
            else
            {
                return BadRequest(new { success = false, message = "回報失敗。" });
            }
        }
        [HttpGet]//取得問題列表，根據問題類型和描述來篩選
        public async Task<IActionResult> GetAllReports([FromQuery] string? questionType, [FromQuery] string? descript)
        {
            //同時把兩個參數傳給Service，如果前端沒傳就會是null
            var result = await _problemService.GetAllReportsAsync(questionType, descript);
            return Ok(result);
        }

        //取得單筆問題明細
        [HttpGet("{pid}")]
        public async Task<IActionResult> GetReportDetail(int pid)
        {
            var result = await _problemService.GetReportDetailAsync(pid);

            if (result == null)
            {
                return NotFound(new { message = "找不到該問題單" });
            }

            return Ok(result);
        }

        //更新問題處理狀態
        [HttpPut("{pid}/status")]
        public async Task<IActionResult> UpdateStatus(int pid, [FromBody] UpdateStatusDto request)
        {
            var success = await _problemService.UpdateStatusAsync(pid, request.Status);

            if (!success)
            {
                return BadRequest(new { message = "更新失敗，找不到該問題單號" });
            }

            return Ok(new { message = "狀態更新成功" });
        }
    }


}
public class UpdateStatusDto
{
    public bool Status { get; set; }
}
