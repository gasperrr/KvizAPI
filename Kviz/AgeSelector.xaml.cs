using CommunityToolkit.Maui.Views;

namespace Kviz;

public partial class AgeSelector : Popup
{
    private int NumOfQuestions;

    public AgeSelector()
    {

        InitializeComponent();
    }

    private async void OnPionirClicked(object sender, EventArgs e)
    {
        NumOfQuestions = 25;
        Close(NumOfQuestions);
    }
    private async void OnMladinecClicked(object sender, EventArgs e)
    {
        NumOfQuestions = 50;
        Close(NumOfQuestions);
    }
    private async void OnPripravnikClicked(object sender, EventArgs e)
    {
        NumOfQuestions = 70;
        Close(NumOfQuestions);
    }

}