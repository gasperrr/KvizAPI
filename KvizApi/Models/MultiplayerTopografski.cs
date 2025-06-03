namespace KvizApi.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        public string HostPlayerId { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
        public Dictionary<string, string> CurrentAnswers { get; set; } = new(); // PlayerId -> Answer
        public Dictionary<string, int> Scores { get; set; } = new();
        public int CurrentQuestionIndex { get; set; } = 0;
        public RoomStatus Status { get; set; } = RoomStatus.Waiting;
    }
    public enum RoomStatus { Waiting, Playing, Finished }

    public class Player
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    
}
