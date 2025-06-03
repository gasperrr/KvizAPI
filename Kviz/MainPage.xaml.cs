using CommunityToolkit.Maui.Views;

namespace Kviz
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnTopografskiTapped(object sender, EventArgs e)
        {

            var Age_Select = new AgeSelector();

            var result = await this.ShowPopupAsync(Age_Select);

            if (result != null)
            {
                var TopografskiZnakiPage = new TopografskiZnakiPage
                {
                    Age = (int)result
                };

                await Navigation.PushAsync(TopografskiZnakiPage);
            }
        }
        private async void OnPoisciBesedeTapped(object sender, EventArgs e)
        {

            var Age_Select = new AgeSelector();

            var result = await this.ShowPopupAsync(Age_Select);

            if (result != null)
            {
                var PoisciBesedePage = new PoisciBesedePage
                {
                    Age = (int)result
                };

                await Navigation.PushAsync(PoisciBesedePage);
            }
        }
        private async void OnMultiplayerTopografskiTapped(object sender, EventArgs e)
        {



            var MultiplayerLobbyPage = new MultiplayerLobbyPage();



                await Navigation.PushAsync(MultiplayerLobbyPage);
            
        }

    }

}
