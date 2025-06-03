using Newtonsoft.Json;
using KvizApi.Models;

namespace KvizApi.Services
{
    public class MultiplayerTopografskiService
    {
        public async Task<List<Question>> FetchQuestionsAsync()
        {
            using var client = new HttpClient();
            var json = await client.GetStringAsync("https://kvizapi.onrender.com/api/TopografskiZnaki");
            return JsonConvert.DeserializeObject<List<Question>>(json) ?? new List<Question>();
        }
    }
}
