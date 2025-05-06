using Microsoft.AspNetCore.Mvc;
using KvizApi.Services; // adjust if your namespace differs
using KvizApi.Models;

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

        // GET api/quiz/questions
        [HttpGet()]
        public ActionResult<IEnumerable<Question>> GetAllQuestions()
        {
            // Fetch all questions from the service and return them

            Console.WriteLine("GetAllQuestions endpoint was hit.");
            var questions = _questionService.GetAllQuestions();
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