using CommunityToolkit.Maui.Views;

namespace Kviz;

public partial class AgeSelector : Popup
{
    private int Age;

    public AgeSelector()
    {

        InitializeComponent();
    }

    private async void OnPionirClicked(object sender, EventArgs e)
    {
        Age = 1;
        Close(Age);
    }
    private async void OnMladinecClicked(object sender, EventArgs e)
    {
        Age = 2;
        Close(Age);
    }
    private async void OnPripravnikClicked(object sender, EventArgs e)
    {
        Age = 3;
        Close(Age);
    }

}