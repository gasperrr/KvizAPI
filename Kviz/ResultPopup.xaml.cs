using CommunityToolkit.Maui.Views;

namespace Kviz;

public partial class ResultPopup : Popup
{
    public ResultPopup(double correctCount, double questionsCount, int score)
    {

        InitializeComponent();

        TitleLabel.Text = "Konec!";
        TitleLabel.TextColor = Colors.Green;

        MessageLabel.Text = $"Od {questionsCount} vprasanj si na {correctCount} odgovil pravilno in dobil {score} tock. \n Bravo!";

    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}
