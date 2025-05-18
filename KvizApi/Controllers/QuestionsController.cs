using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using KvizApi.Models;
using System.Text.Json;

namespace KvizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Question>>> GetAllQuestions()
        {
            // Path to the questions.json file in the wwwroot folder
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Data", "questions.json");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Questions file not found.");
            }

            // Read the content of the file
            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);

            // Deserialize the JSON data into a list of Question objects
            //var questions = JsonConvert.DeserializeObject<List<Question>>(jsonData);
            var questions = JsonSerializer.Deserialize<List<Question>>(jsonData);

            if (questions == null || questions.Count == 0)
            {
                return NotFound("No questions available.");
            }

            return Ok(questions);
        }

    }
}