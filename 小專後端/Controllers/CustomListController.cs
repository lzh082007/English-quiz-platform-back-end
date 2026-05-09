using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using 小專後端.Dtos;
using 小專後端.Dtos.list;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomListController : ControllerBase
    {
        private readonly CustomListService _customListService;

        public CustomListController(CustomListService customListService)
        {
            _customListService = customListService;
        }

        //新增單字到清單
        [HttpPost("{listId}/word")]
        public async Task<IActionResult> AddWordToList(int listId,[FromBody] AddWordToListDto request)
        {
            var success = await _customListService.AddWordToListAsync(listId, request.WordId);

            if (!success) return BadRequest(new { message = "這個單字已經在清單中了" });

            return Ok(new { message = "成功加入清單" });
        }

        //取得使用者的所有清單從Toke抓
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomListDto>>> GetUserLists()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Token無效或找不到使用者ID" });
            }

            var lists = await _customListService.GetUserListsAsync(userId);
            return Ok(lists);
        }

        //取得清單內的單字
        [HttpGet("{listId}/words")]
        public async Task<ActionResult<IEnumerable<WordDto>>> GetWordsInList(
            int listId,
            [FromQuery] byte? difficultyLevel)
        {
            var words = await _customListService.GetListWordsAsync(listId, difficultyLevel);
            return Ok(words);
        }
        [HttpPost]
        public async Task<IActionResult> CreateList([FromBody] CreateListDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Token無效或找不到使用者ID" });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "清單名稱不能為空" });
            }

            var newListId = await _customListService.CreateListAsync(userId, request.Title, request.Description);

            return Ok(new
            {
                message = "清單建立成功",
                listId = newListId
            });
        }
        //編輯清單
        [HttpPut("{listId}")]
        public async Task<IActionResult> UpdateList(int listId, [FromBody] UpdateListDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var success = await _customListService.UpdateListAsync(listId, userId, request.Title, request.Description);
            if (!success) return NotFound(new { message = "找不到該清單或無權限修改" });

            return Ok(new { message = "更新成功" });
        }
        //刪除清單
        [HttpDelete("{listId}")]
        public async Task<IActionResult> DeleteList(int listId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var success = await _customListService.DeleteListAsync(listId, userId);
            if (!success) return NotFound(new { message = "找不到該清單或無權限刪除" });

            return Ok(new { message = "清單已刪除" });
        }
        //刪除清單單字
        [HttpDelete("{listId}/word/{wordId}")]
        public async Task<IActionResult> RemoveWord(int listId, int wordId)
        {
            var success = await _customListService.RemoveWordFromListAsync(listId, wordId);
            if (!success) return NotFound(new { message = "清單中找不到此單字" });

            return Ok(new { message = "單字已移除" });
        }
    }
}