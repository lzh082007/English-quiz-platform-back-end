using Microsoft.AspNetCore.Mvc;
using 小專後端.Dtos;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly DictionaryService _dictionaryService;

        public DictionaryController(DictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }
    
        [HttpGet("categories")]//取得單字分類
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var result = await _dictionaryService.GetCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("words")]//取得單字列表，根據類別ID和難度等級來篩選
        public async Task<ActionResult<IEnumerable<WordDto>>> GetWords(
            [FromQuery] int categoryId,
            [FromQuery] byte difficultyLevel)
        {
            var result = await _dictionaryService.GetWordsAsync(categoryId, difficultyLevel);
            return Ok(result);
        }
    }
}

