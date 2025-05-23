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
                var KvizPage = new KvizPage
                {
                    Age = (int)result
                };

                await Navigation.PushAsync(KvizPage);
            }
        }
        private async void OnKrizankaTapped(object sender, EventArgs e)
        {

            var Age_Select = new AgeSelector();

            var result = await this.ShowPopupAsync(Age_Select);

            if (result != null)
            {
                var KrizankaPage = new KrizankaPage
                {
                    Age = (int)result
                };

                await Navigation.PushAsync(KrizankaPage);
            }
        }

    }

}
