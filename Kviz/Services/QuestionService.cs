using Kviz.Models;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Kviz.Services
{
    public class QuestionService
    {
        private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "questions.txt");

        // Method to read questions from the file and return them
        public List<Question> GetQuestionsFromFile()
        {
            var fileContent = File.ReadAllText(_filePath);
            var questions = JsonConvert.DeserializeObject<List<Question>>(fileContent);
            return questions;
        }

        // Method to get 10 random questions with randomized options
        public List<Question> GetRandomQuestions()
        {
            var questions = GetQuestionsFromFile();
            var randomQuestions = new List<Question>();

            foreach (var question in questions.OrderBy(q => Guid.NewGuid()).Take(10))
            {
                // Get the correct answer for the current question
                var correctAnswer = question.CorrectAnswer;

                // Get a list of other correct answers to use as wrong answers
                var wrongAnswers = questions
                    .Where(q => q.Id != question.Id)  // Exclude the current question
                    .Select(q => q.CorrectAnswer)
                    .OrderBy(_ => Guid.NewGuid())     // Shuffle the wrong answers
                    .Take(3)                         // Select only 3 wrong answers
                    .ToList();

                // Combine correct answer with the wrong answers
                var allAnswers = wrongAnswers.Append(correctAnswer).OrderBy(_ => Guid.NewGuid()).ToList();

                // Update the current question's options with randomized answers
                randomQuestions.Add(new Question
                {
                    Id = question.Id,
                    QuestionText = question.QuestionText,
                    CorrectAnswer = correctAnswer,
                    Options = allAnswers // Add the randomized options here
                });
            }

            return randomQuestions;
        }
    }
}
