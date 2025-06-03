using Kviz.Services;

namespace Kviz;

public partial class MultiplayerLobbyPage : ContentPage
{
	public MultiplayerLobbyPage()
	{
		InitializeComponent();
	}

    private readonly MultiplayerService _multiplayerService = new();

    private async void OnCreateRoomClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Error", "Enter your name", "OK");
            return;
        }

        var roomId = await _multiplayerService.CreateRoomAsync(name);
        var playerId = await _multiplayerService.JoinRoomAsync(roomId, name); // Host joins too

        await Navigation.PushAsync(new GameLobbyPage(roomId, playerId, name));
    }

    private async void OnJoinRoomClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var roomCode = RoomCodeEntry.Text?.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(roomCode))
        {
            await DisplayAlert("Error", "Enter both name and room code", "OK");
            return;
        }

        var playerId = await _multiplayerService.JoinRoomAsync(roomCode, name);
        await Navigation.PushAsync(new GameLobbyPage(roomCode, playerId, name));
    }
}