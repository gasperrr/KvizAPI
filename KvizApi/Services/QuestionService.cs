using KvizApi.Models;

namespace KvizApi.Services
{
    public class QuestionService
    {
        private List<Question> _questions;

        public QuestionService()
        {
            _questions = new List<Question>
            {
                new Question { Id = 1, QuestionText = "What is the capital of France?", CorrectAnswer = "Paris" },
                new Question { Id = 2, QuestionText = "Which planet is closest to the Sun?", CorrectAnswer = "Mercury" },
                new Question { Id = 3, QuestionText = "What is the largest ocean on Earth?", CorrectAnswer = "Pacific" },
                new Question { Id = 4, QuestionText = "Which country is known as the Land of the Rising Sun?", CorrectAnswer = "Japan" },
                new Question { Id = 5, QuestionText = "What is the capital of Japan?", CorrectAnswer = "Tokyo" },
                new Question { Id = 6, QuestionText = "What is the speed of light?", CorrectAnswer = "299,792 km/s" },
                new Question { Id = 7, QuestionText = "What is the largest mammal?", CorrectAnswer = "Whale" },
                new Question { Id = 8, QuestionText = "Who invented the telephone?", CorrectAnswer = "Alexander Graham Bell" },
                new Question { Id = 9, QuestionText = "What is the tallest mountain on Earth?", CorrectAnswer = "Mount Everest" },
                new Question { Id = 10, QuestionText = "Which is the largest planet in our solar system?", CorrectAnswer = "Jupiter" }
            };

            // Generate options for each question
            foreach (var q in _questions)
            {
                var incorrect = _questions
                    .Where(x => x.Id != q.Id)
                    .Select(x => x.CorrectAnswer)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(3)
                    .ToList();

                incorrect.Add(q.CorrectAnswer);
                q.Options = incorrect.OrderBy(_ => Guid.NewGuid()).ToList(); // Shuffle all 4 options
            }
        }

        public List<Question> GetQuestions(int count = 10)
        {
            var rnd = new Random();
            return _questions.OrderBy(q => rnd.Next()).Take(count).ToList();
        }

        // Method to get all questions
        public List<Question> GetAllQuestions()
        {
            return _questions;
        }

        // Method to get a question by its ID
        public Question GetQuestionById(int id)
        {
            return _questions.FirstOrDefault(q => q.Id == id);
        }
    }
}