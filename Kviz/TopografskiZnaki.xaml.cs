using System.Timers;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;
using System.Net.Http;
using Kviz.Models;
using System.Buffers.Text;
using CommunityToolkit.Maui.Views;

namespace Kviz
{
    public partial class TopografskiZnakiPage : ContentPage
    {
        public int Age { get; set; }

        private const double TotalTime = 20000;
        private double RefreshFrequency = 60;
        private double timeLeft = TotalTime;
        private System.Timers.Timer quizTimer;
        private int NumOfQuestions;

        private int questionCount = 0;
        private int correctCount = 0;
        private List<Question> questions = new();
        private List<Question> answers = new();
        private string correctAnswer = string.Empty;

        private int score = 0;

        public TopografskiZnakiPage()
        {
            InitializeComponent();

            quizTimer = new System.Timers.Timer();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadingOverlay.IsVisible = true;
            await LoadQuestions();  // Fetch the questions when the page appears
            LoadingOverlay.IsVisible = false;
            ShuffleAndTrimQuestions();
            LoadQuestion(); // First question
        }

        private async Task LoadQuestions()
        {

            try
            {

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                string apiUrl = "https://kvizapi.onrender.com/api/TopografskiZnaki";

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = new TimeSpan(0, 1, 0);
                    System.Diagnostics.Debug.WriteLine("Requesting API...");
                    var response = await client.GetStringAsync(apiUrl);
                    System.Diagnostics.Debug.WriteLine("Response received!");

                    Console.WriteLine(response); // Or use Debug.WriteLine or display in alert
                    questions = JsonConvert.DeserializeObject<List<Question>>(response) ?? new List<Question>();


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
            if (Age == 1)
            {
                NumOfQuestions = 25;
            }
            else if (Age == 2)
            {
                NumOfQuestions = 50;
            }
            else if(Age == 3)
            {
                NumOfQuestions = 70;
            }
            questions = questions.Take(NumOfQuestions).ToList();
            answers = questions.OrderBy(q => rnd.Next()).ToList();
            questions = questions.OrderBy(q => rnd.Next()).Take(10).ToList();

            foreach (var q in questions)
            {
                var distractors = answers
                        .Where(x => x.Id != q.Id && x.Tags != null && x.Tags.Any(tag => q.Tags.Contains(tag)))
                        .Select(x => x.CorrectAnswer)
                        .Distinct()
                        .Where(ans => ans != q.CorrectAnswer)
                        .OrderBy(x => rnd.Next())
                        .Take(3)
                        .ToList();

                // Combine correct + distractors and shuffle
                distractors.Add(q.CorrectAnswer);
                q.Options = distractors.OrderBy(_ => rnd.Next()).ToList();

                distractors.Clear();
            }
        }

        private void LoadQuestion()
        {
            if (questionCount < 10)
                VprasanjeText.Text = $"Vprasanje - {questionCount + 1}/{questions.Count}";
            else
                VprasanjeText.Text = $"Vprasanje - {questionCount}/{questions.Count}";

            if (questionCount >= questions.Count)  // Ensure this is the last question
            {
                ShowResults();
                quizTimer.Stop();  // Stop the timer after the last question
                return;
            }

            var question = questions[questionCount];
            var options = question.Options;

            string baseUrl = "https://kvizapi.onrender.com";

            QuestionImage.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = TimeSpan.FromDays(1)
            };

            string imageUrl = $"{baseUrl}/Images/{question.Id}.PNG";
            System.Diagnostics.Debug.WriteLine(imageUrl);

            QuestionImage.Source = ImageSource.FromUri(new Uri(imageUrl));
            //QuestionLabel.Text = question.QuestionText;
            OptionA.Text = options[0];
            OptionB.Text = options[1];
            OptionC.Text = options[2];
            OptionD.Text = options[3];

            correctAnswer = question.CorrectAnswer;

            // Reset button styles
            foreach (var btn in new[] { OptionA, OptionB, OptionC, OptionD })
            {
                btn.BackgroundColor = Colors.SlateBlue;
                btn.TextColor = Colors.Black;
                btn.IsEnabled = true;
            }
            timeLeft = TotalTime;

            StartTimer();
            // Disable all buttons to prevent double-tapping
            OptionA.IsEnabled = OptionB.IsEnabled = OptionC.IsEnabled = OptionD.IsEnabled = true;
        }

        private void StartTimer()
        {
            quizTimer = new System.Timers.Timer(1000 / RefreshFrequency); // updates per second
            quizTimer.Elapsed += OnTimerElapsed;
            quizTimer.Start();
        }
        private void StopTimer()
        {
            quizTimer.Stop();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            timeLeft -= (1000 / RefreshFrequency);
            if (questionCount >= questions.Count)  // Ensure this is the last question
            {
                quizTimer.Stop();
            }
            MainThread.BeginInvokeOnMainThread(async () =>
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
                    
                    var Popup = new AnswerPopup(false, "Zmanjkalo_casa");
                    await this.ShowPopupAsync(Popup);
                    if (questionCount < questions.Count)
                    {
                        questionCount++;
                        LoadQuestion();
                    }
                    else
                    {
                        ShowResults();
                    }
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
            StopTimer();
            var button = sender as Button;
            var selectedAnswer = button?.Text;
            int timeBonus;

            // Disable all buttons to prevent double-tapping
            OptionA.IsEnabled = OptionB.IsEnabled = OptionC.IsEnabled = OptionD.IsEnabled = false;

            // Color all buttons: green if correct, red if selected and wrong
            foreach (var btn in new[] { OptionA, OptionB, OptionC, OptionD })
            {
                if (btn.Text == correctAnswer)
                {
                    btn.BackgroundColor = Colors.Green;
                    btn.TextColor = Colors.White;
                }
                else if (btn == button)
                {
                    btn.BackgroundColor = Colors.Red;
                    btn.TextColor = Colors.White;
                }
            }

            bool isCorrect = selectedAnswer == correctAnswer;

            if (isCorrect)
            {
                correctCount++;

                // Calculate bonus: scale 20s to 1000 pts
                timeBonus = (int)((timeLeft / 20000.0) * 1000);
                score += timeBonus;
            }
            else
                timeBonus = 0;
            // Show feedback immediately
            string feedback = isCorrect ? "Pravilno!" : "Napaèno";
            ScoreLabel.Text = $"Tocke: {score}";


            var Popup = new AnswerPopup(isCorrect, $"+ {timeBonus}");
            await this.ShowPopupAsync(Popup);


            // Short delay so user sees color before moving on
            await Task.Delay(1000);

            questionCount++;

            LoadingOverlay.IsVisible = true;
            LoadQuestion(); // Show next
            LoadingOverlay.IsVisible = false;

        }

        private async void ShowResults()
        {
            double percentage = (double)correctCount / questions.Count * 100;

            var Popup = new ResultPopup(correctCount, questions.Count,score);
            await this.ShowPopupAsync(Popup);

            await Navigation.PopAsync(); // Go back to main page
        }

    }
}