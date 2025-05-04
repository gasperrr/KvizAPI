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
            await Navigation.PushAsync(new KvizPage());
        }

        //private async void OnMediumTapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new MediumPage());
        //}

        //private async void OnHardTapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new HardPage());
        //}

    }

}
