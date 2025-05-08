using System.Timers;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;
using System.Net.Http;
using Kviz.Models;
using Kviz.Services;
//using System.Net.Http.Json;

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
            
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadingOverlay.IsVisible = true;
            await LoadQuestions();  // Fetch the questions when the page appears
            LoadingOverlay.IsVisible = false;
            ShuffleAndTrimQuestions();
            LoadQuestion(); // First question
            StartTimer();
        }

        private async Task LoadQuestions()
        {

            try
            {

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = new TimeSpan(0, 1, 0);
                    System.Diagnostics.Debug.WriteLine("Requesting API...");
                    
                    var response = await client.GetStringAsync("https://10.0.2.2:7169/api/questions");
                    System.Diagnostics.Debug.WriteLine("Response received!");

                    Console.WriteLine(response); // Or use Debug.WriteLine or display in alert
                    questions = JsonConvert.DeserializeObject<List<Question>>(response);


                    if (questions == null || questions.Count == 0)
                    {
                        await DisplayAlert("Error", "No questions returned from the API.", "OK");
                        return;
                    }

                    // Reset counters and load the first question
                    questionCount = 0;
                    correctCount = 0; // Load the first question
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP specific issues
                await DisplayAlert("Error", $"Network error: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
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
            LoadingOverlay.IsVisible = true;
            LoadQuestion(); // Show next
            LoadingOverlay.IsVisible = false;

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