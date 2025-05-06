using System.Timers;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;
using System.Net.Http;
using Kviz.Models;
using Kviz.Services;

namespace Kviz
{
    public partial class KvizPage : ContentPage
    {
        private const double TotalTime = 60000; // 60 seconds
        private double timeLeft = TotalTime;
        private System.Timers.Timer quizTimer;

        private int questionCount = 0;
        private int correctCount = 0;
        private List<Question> questions = new();
        private string correctAnswer;

        public KvizPage()
        {
            InitializeComponent();
            LoadQuestions();
            ShuffleAndTrimQuestions();
            LoadQuestion(); // First question
            StartTimer();
        }

        // Load and parse the embedded JSON file
        //private void LoadQuestions()
        //{
        //    var assembly = typeof(KvizPage).Assembly;

        //    using Stream stream = assembly.GetManifestResourceStream("Kviz.Resources.Raw.questions.txt");
        //    using StreamReader reader = new StreamReader(stream);
        //    string fileContent = reader.ReadToEnd();

        //    questions = JsonConvert.DeserializeObject<List<Question>>(fileContent);
        //}

        private async void LoadQuestions()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://localhost:7169/api/questions");
                    questions = JsonConvert.DeserializeObject<List<Question>>(response);

                    if (questions == null || questions.Count == 0)
                    {
                        await DisplayAlert("Error", "No questions returned from the API.", "OK");
                        return;
                    }

                    // Reset counters and load the first question
                    questionCount = 0;
                    correctCount = 0;
                    LoadQuestion(); // Load the first question
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load questions: {ex.Message}", "OK");
            }
        }




        // Shuffle questions and build 4 options per question
        private void ShuffleAndTrimQuestions()
        {
            var rnd = new Random();

            // Shuffle questions and pick 10
            questions = questions.OrderBy(q => rnd.Next()).Take(10).ToList();

            foreach (var q in questions)
            {
                // Select 3 random correct answers from *other* questions
                var distractors = questions
                    .Where(x => x.CorrectAnswer != q.CorrectAnswer)
                    .Select(x => x.CorrectAnswer)
                    .Distinct()
                    .OrderBy(x => rnd.Next())
                    .Take(3)
                    .ToList();

                // Combine correct + distractors and shuffle
                distractors.Add(q.CorrectAnswer);
                q.Options = distractors.OrderBy(_ => rnd.Next()).ToList();
            }
        }

        private void LoadQuestion()
        {
            if (questionCount >= 10)
            {
                ShowResults();
                return;
            }

            var question = questions[questionCount];
            var options = question.Options;

            QuestionLabel.Text = question.QuestionText;
            OptionA.Text = options[0];
            OptionB.Text = options[1];
            OptionC.Text = options[2];
            OptionD.Text = options[3];

            correctAnswer = question.CorrectAnswer;
        }

        private void StartTimer()
        {
            quizTimer = new System.Timers.Timer(50); // 20 updates per second
            quizTimer.Elapsed += OnTimerElapsed;
            quizTimer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timeLeft -= 50;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText.Text = ((int)timeLeft / 1000).ToString();
                double progress = timeLeft / TotalTime;
                TimerBar.WidthRequest = TimerBarContainer.Width * progress;
                TimerBar.BackgroundColor = GetColorGradient(progress);

                if (timeLeft <= 0)
                {
                    quizTimer.Stop();
                    TimerText.Text = "0";
                    TimerBar.WidthRequest = 0;
                    DisplayAlert("Time's Up!", "You ran out of time.", "OK");
                    ShowResults();
                }
            });
        }

        private Color GetColorGradient(double progress)
        {
            int r = (int)(213 * (1 - progress)); // red increases as time decreases
            int g = (int)(200 * progress);       // green decreases
            return Color.FromRgb(r, g, 0);
        }

        private async void OnAnswerClicked(object sender, EventArgs e)
        {
            quizTimer.Stop(); // Optional: pause on answer

            var button = sender as Button;
            var selectedAnswer = button.Text;

            if (selectedAnswer == correctAnswer)
                correctCount++;


            questionCount++;
            LoadQuestion(); // Show next

            quizTimer.Start(); // Optional: resume
        }

        private async void ShowResults()
        {
            double percentage = (double)correctCount / 10 * 100;
            await DisplayAlert("Quiz Finished", $"Your score: {percentage:F1}%", "OK");
            await Navigation.PopAsync(); // Return to main page
        }

        private readonly ApiService _apiService = new ApiService();
    }
}