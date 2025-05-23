using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using KvizApi.Models;
using System.Text.Json;

namespace KvizApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class PoisciBesedeController : ControllerBase
    {
        [HttpGet("PoisciBesede")]
        public async Task<ActionResult<IEnumerable<string>>> GetCrosswordWords()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Data", "PoisciBesede.json");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Words file not found.");

            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);

            var words = JsonSerializer.Deserialize<List<string>>(jsonData)?
                .Select(w => w.Trim().ToUpper())
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Distinct()
                .ToList();

            if (words == null || words.Count == 0)
                return NotFound("No words available.");

            return Ok(words);
        }
    }
}