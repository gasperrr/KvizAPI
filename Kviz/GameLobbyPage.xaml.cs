using Kviz.Models;
using Kviz.Services;

namespace Kviz;

public partial class GameLobbyPage : ContentPage
{
    private readonly string _roomId;
    private readonly string _playerId;
    private readonly string _playerName;
    private readonly MultiplayerService _multiplayerService = new();

    private readonly List<Player> _players = new();

    public GameLobbyPage(string roomId, string playerId, string playerName)
    {
        InitializeComponent();

        _roomId = roomId;
        _playerId = playerId;
        _playerName = playerName;

        RoomCodeLabel.Text = $"Room: {roomId}";
        PlayersListView.ItemsSource = _players;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(2), () =>
        {
            _ = RefreshRoomStateAsync(); // fire-and-forget
            return true; // return true to keep the timer running
        });
    }

    private async Task RefreshRoomStateAsync()
    {
        try
        {
            var room = await _multiplayerService.GetRoomStateAsync(_roomId);

            _players.Clear();
            foreach (var player in room.Players)
                _players.Add(player);

            PlayersListView.ItemsSource = null;
            PlayersListView.ItemsSource = _players;

            StartGameButton.IsVisible = _playerId == room.HostPlayerId && room.Players.Count > 1;
        }
        catch
        {
            // Optional: display error or ignore
        }
    }

    private async void OnStartGameClicked(object sender, EventArgs e)
    {
        // You can implement starting logic here (e.g., changing status on server)
        await DisplayAlert("Start Game", "Game would start now!", "OK");
    }
}
