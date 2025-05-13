using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using KvizApi.Services; // adjust if your namespace differs
using KvizApi.Models;
using System.Text.Json;

namespace KvizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly QuestionService _questionService;

        // Constructor that takes the QuestionService
        public QuestionsController(QuestionService questionService)
        {
            _questionService = questionService;
        }

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

        // GET api/quiz/questions/{id}
        [HttpGet("{id}")]
        public ActionResult<Question> GetQuestionById(int id)
        {
            // Fetch a single question by ID
            var question = _questionService.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }
    }
}