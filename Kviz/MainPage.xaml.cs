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
        
    }

}
