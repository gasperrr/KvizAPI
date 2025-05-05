namespace Kviz.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> Options { get; set; } // Add this property to hold the options
    }
}
