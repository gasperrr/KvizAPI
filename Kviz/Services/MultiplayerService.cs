using Kviz.Models;
using Newtonsoft.Json;


namespace Kviz.Services;

public class MultiplayerService
{
    private readonly HttpClient _http = new();

    public async Task<string> CreateRoomAsync(string hostName)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync($"https://kvizapi.onrender.com/room/create?hostName={hostName}", null);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create room");

        var roomId = await response.Content.ReadAsStringAsync();
        return roomId.Trim('"'); // Remove quotes from string
    }

    public async Task<string> JoinRoomAsync(string roomId, string playerName)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync($"https://kvizapi.onrender.com/room/join?roomId={roomId}&playerName={playerName}", null);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to join room");

        var playerId = await response.Content.ReadAsStringAsync();
        return playerId.Trim('"');
    }


    public async Task<Room> GetRoomStateAsync(string roomId)
    {
        using var client = new HttpClient();
        var json = await client.GetStringAsync($"https://kvizapi.onrender.com/room/{roomId}/state");

        return JsonConvert.DeserializeObject<Room>(json) ?? new Room();
    }


    public async Task SubmitAnswerAsync(string roomId, string playerId, string answer)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync($"https://kvizapi.onrender.com/room/{roomId}/answer?playerId={playerId}&answer={answer}", null);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to submit answer");
    }


    public async Task<ResultsResponse> GetResultsAsync(string roomId)
    {
        using var client = new HttpClient();
        var json = await client.GetStringAsync($"https://kvizapi.onrender.com/room/{roomId}/results");

        return JsonConvert.DeserializeObject<ResultsResponse>(json) ?? new ResultsResponse();
    }


    public async Task<List<FinalScore>> GetFinalResultsAsync(string roomId)
    {
        using var client = new HttpClient();
        var json = await client.GetStringAsync($"https://kvizapi.onrender.com/room/{roomId}/final");
        return JsonConvert.DeserializeObject<List<FinalScore>>(json) ?? new List<FinalScore>();
    }

    private async Task<List<Question>> FetchQuestionsAsync()
    {
        using var client = new HttpClient();
        var json = await client.GetStringAsync("https://kvizapi.onrender.com/api/Topografski");
        return JsonConvert.DeserializeObject<List<Question>>(json) ?? new List<Question>();
    }

}