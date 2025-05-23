using CommunityToolkit.Maui.Views;

namespace Kviz;

public partial class AnswerPopup : Popup
{
    public AnswerPopup(bool isCorrect, string message)
    {
        
        InitializeComponent();
        if (isCorrect)
        {
            TitleLabel.Text = "Pravilno!";
            TitleLabel.TextColor = Colors.Green;
        }
        else
        {
            TitleLabel.Text = "Napaèno!";
            TitleLabel.TextColor = Colors.Red;
        }
        MessageLabel.Text = message;
        if(message == "Zmanjkalo_casa")
        {
            TitleLabel.Text = "Zmanjkalo ti je èasa!";
            TitleLabel.TextColor = Colors.Red;
            MessageLabel.Text = "";
        }

        _ = AnimateAfterShown(isCorrect);
    }
    private async Task AnimateAfterShown(bool isCorrect)
    {
        await Task.Delay(100); // let layout render

        if (isCorrect)
            await AnimateCorrectAsync();
        else
            await AnimateWrongAsync();
    }
    public async Task AnimateCorrectAsync()
    {
        
        for (int i = 0; i < 4; i++)
        {
            await AnsPopup.ScaleTo(1.1, 100);
            await AnsPopup.ScaleTo(1.0, 100);
        }
    }
    public async Task AnimateWrongAsync()
    {
        double originalX = AnsPopup.TranslationX;

        for (int i = 0; i < 4; i++)
        {
            await AnsPopup.TranslateTo(originalX - 10, 0, 50);
            await AnsPopup.TranslateTo(originalX + 10, 0, 50);
        }

        await AnsPopup.TranslateTo(originalX, 0, 50);
    }
    private async void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}
