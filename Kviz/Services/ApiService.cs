using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kviz.Models;

namespace Kviz.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7169"); // Replace with your actual API URL
        }

        public async Task<List<Question>> GetQuestionsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Question>>("api/questions/questions");
        }
    }
}