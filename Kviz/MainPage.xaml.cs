using CommunityToolkit.Maui.Views;

namespace Kviz
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnKvizTapped(object sender, EventArgs e)
        {

            var Age_Select = new AgeSelector();

            var result = await this.ShowPopupAsync(Age_Select);

            if (result != null)
            {
                var KvizPage = new KvizPage
                {
                    NumOfQuestions = (int)result
                };

                await Navigation.PushAsync(KvizPage);
            }
            //else
            //{
            //    result = 0;
            //}
        }

    }

}
