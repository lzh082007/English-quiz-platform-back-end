using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using 小專後端.Dtos;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    public class PractiseController : ControllerBase
    {
        [Authorize] //練習功能一樣需要登入
        [Route("api/[controller]")]
        [ApiController]
        public class PracticeController : ControllerBase
        {
            private readonly PractiseService _practiceService;

            public PracticeController(PractiseService practiceService)
            {
                _practiceService = practiceService;
            }

            //取得自訂清單的翻卡練習單字
            [HttpGet("flipcard/list/{listId}")]
            public async Task<ActionResult<IEnumerable<WordDto>>> GetFlipCardForList(int listId)
            {
                var words = await _practiceService.GetCustomListAsync(listId, 20);

                return Ok(words);
            }
        }
    }
}

