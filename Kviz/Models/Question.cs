namespace Kviz.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public string ImagePath { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
}
