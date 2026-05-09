using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExcelController : ControllerBase
    {
        private readonly ExcellService _excellService;
        public ExcelController(ExcellService excellService)
        {
            _excellService = excellService;
        }
        //匯入單字
        [HttpPost("Words")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("請選則上傳的檔案");
            }
            var extension = Path.GetExtension(file.FileName).ToLower();
            if(extension != ".xlsx")
            {
                return BadRequest("只支援.xlsx檔案格式");
            }
            try
            {
                // 回傳的錯誤訊息清單接起來
                var duplicateMessages = await _excellService.ProcessWordExcelAsync(file);

                // 判斷有沒有重複的單字
                if (duplicateMessages.Any())
                {
                    //把清單傳給前端
                    return Ok(new
                    {
                        message = "匯入完成，單字重複。",
                        duplicates = duplicateMessages 
                    });
                }

                return Ok(new
                {
                    success = true,
                    hasWarning = false,
                    message = "單字匯入成功"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"匯入錯誤:{ex.Message}");
            }
        }
        //匯出單字
        [HttpGet("Words")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var fileBytes = await _excellService.ExportWordsToExcelAsync();
                var fileName = $"WordsExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                // 回傳檔案下載的MIMEtype
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"匯出錯誤: {ex.Message}");
            }
        }
        //模板下載
        [HttpGet("Template")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                
                var fileBytes = _excellService.GetExcelTemplate();
                var fileName = "匯入單字模板.xlsx"; 

                // 檔案下載
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"下載模板錯誤: {ex.Message}");
            }
        }
        //上傳題目S
        [HttpPost("PushQuestions")] 
        public async Task<IActionResult> ImportQuestionExcel(IFormFile file) 
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("請選擇上傳的檔案");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx")
            {
                return BadRequest("只支援.xlsx檔案格式");
            }

            try
            {
                var duplicateMessages = await _excellService.QuestionExcelImpot(file);

                if (duplicateMessages.Any())
                {
                    return Ok(new
                    {
                        message = "匯入完成但有些題目重複。", 
                        duplicates = duplicateMessages
                    });
                }

                return Ok(new
                {

                    message = "題目匯入成功" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"匯入錯誤:{ex.Message}");
            }
        }

        [HttpGet("Questions")]//匯出題目
        public async Task<IActionResult> ExportQuestionsExcel()
        {
            try
            {
                var fileBytes = await _excellService.OputQuestions();
                var fileName = $"QuestionsExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"匯出錯誤: {ex.Message}");
            }
        }

        // 題目模板下載
        [HttpGet("QuestionTemplate")]
        public IActionResult DownloadQuestionTemplate()
        {
            try
            {
                var fileBytes = _excellService.GetQuestionExcelTemplate();
                var fileName = "匯入題目模板.xlsx";

                // 檔案下載
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"下載模板錯誤: {ex.Message}");
            }
        }
    }
}
