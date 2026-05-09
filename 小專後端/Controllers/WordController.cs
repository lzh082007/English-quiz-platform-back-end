using Microsoft.AspNetCore.Mvc;
using 小專後端.Dtos;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly WordService _WordService;
        public WordController(WordService WordService)
        {
            _WordService = WordService;
        }
        [HttpPut("{wid}")]//更新單字

        public async Task<IActionResult> UpdateWord(int wid, [FromBody] WordUpdateDto dto)
        {
            if (wid != dto.wid)
            {
                return BadRequest(new { message = "資料ID不符" });
            }

            try
            {
                var isUpdated = await _WordService.UpdateSingleWordAsync(wid, dto);

                if (isUpdated)
                {
                    return Ok(new { message = $"單字ID:{wid}更新成功" });
                }
                else
                {
                    return NotFound(new { message = "更新失敗" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新錯誤: {ex.Message}");
            }
        }
        [HttpDelete("{wid}")]//刪除單字
        public async Task<IActionResult> DeleteWord(int wid)
        {
            try
            {
                var isDeleted = await _WordService.DeleteSingleWordAsync(wid);

                if (isDeleted)
                {
                    return Ok(new { message = $"單字ID:{wid}刪除成功" });
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
        [HttpGet]//取得單字列表，根據難度等級和類別ID來篩選
        public async Task<IActionResult> GetWords([FromQuery] byte? difficulty, [FromQuery] int? category_id)
        {
            try
            {
                var words = await _WordService.GetWordsAsync(difficulty, category_id);
                return Ok(words);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"單字列表錯誤:{ex.Message}");
            }
        }
      
    }
}
