using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kviz.Models
{
    public class Room
    {
        public string Id { get; set; } = string.Empty;
        public string HostPlayerId { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new List<Player>();
        public int CurrentQuestionIndex { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
    }

    public class Player
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ResultsResponse
    {
        public string correct { get; set; } = string.Empty;
        public Dictionary<string, int> scores { get; set; } = new Dictionary<string, int>();
    }

    public class FinalScore
    {
        public string playerId { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int score { get; set; } = 0;
    }
}