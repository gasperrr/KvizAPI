using System.Timers;
using Microsoft.Maui.Graphics;

namespace Kviz;

public partial class KvizPage : ContentPage
{
    private const int TotalTime = 60;
    private int timeLeft = TotalTime;
    private System.Timers.Timer quizTimer;

    public KvizPage()
    {
        InitializeComponent();
        StartTimer();
    }

    private void StartTimer()
    {
        quizTimer = new System.Timers.Timer(1000); // tick every second
        quizTimer.Elapsed += OnTimerElapsed;
        quizTimer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        timeLeft--;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update timer text
            TimerText.Text = timeLeft.ToString();

            // Update bar width
            double progress = (double)timeLeft / TotalTime;
            double containerWidth = TimerBarContainer.Width;
            TimerBar.WidthRequest = containerWidth * progress;

            // Color gradient from green to red
            TimerBar.BackgroundColor = GetColorGradient(progress);

            // Stop when time is up
            if (timeLeft <= 0)
            {
                quizTimer.Stop();
                TimerBar.WidthRequest = 0;
                TimerText.Text = "0";
                DisplayAlert("Time's Up!", "You ran out of time.", "OK");
            }
        });
    }

    private Color GetColorGradient(double progress)
    {
        // From green (#00C853) to red (#D50000)
        int r = (int)(213 * (1 - progress)); // red increases as time decreases
        int g = (int)(200 * progress);       // green decreases
        return Color.FromRgb(r, g, 0);
    }

    private async void OnAnswerClicked(object sender, EventArgs e)
    {
        quizTimer.Stop();

        var button = sender as Button;
        var selectedAnswer = button.Text;

        if (selectedAnswer == "Paris")
        {
            await DisplayAlert("Correct!", "That's the right answer.", "Next");
        }
        else
        {
            await DisplayAlert("Wrong!", "That's not correct.", "Try Again");
        }
    }
}
