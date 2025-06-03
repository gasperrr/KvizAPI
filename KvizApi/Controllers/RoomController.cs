using KvizApi.Models;
using KvizApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace KvizApi.Controllers
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly MultiplayerTopografskiService _multiplayerService;
        // In-memory room storage
        private static ConcurrentDictionary<string, Room> Rooms = new();

        public RoomController(MultiplayerTopografskiService multiplayerService)
        {
            _multiplayerService = multiplayerService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateRoom([FromQuery] string hostName)
        {
            var questions = await _multiplayerService.FetchQuestionsAsync();

            var room = new Room
            {
                HostPlayerId = Guid.NewGuid().ToString(),
                Players = new List<Player>
                {
                    new Player { Id = Guid.NewGuid().ToString(), Name = hostName }
                }
            };

            Rooms[room.Id] = room;
            return Ok(room.Id);
        }

        [HttpPost("join")]
        public ActionResult<string> JoinRoom([FromQuery] string roomId, [FromQuery] string playerName)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
                return NotFound("Room not found");

            lock (room) // Lock on room object to avoid simultaneous edits
            {
                if (room.Players.Count >= 32)
                    return BadRequest("Room full");

                if (room.Status != RoomStatus.Waiting)
                    return BadRequest("Game already started");

                var player = new Player { Id = Guid.NewGuid().ToString(), Name = playerName };
                room.Players.Add(player);
                return Ok(player.Id);
            }
        }

        [HttpGet("{roomId}/state")]
        public ActionResult<Room> GetRoomState(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
                return NotFound();

            return Ok(room);
        }

        [HttpPost("{roomId}/answer")]
        public ActionResult SubmitAnswer(string roomId, [FromQuery] string playerId, [FromQuery] string answer)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
                return NotFound();

            if (room.Status != RoomStatus.Playing)
                return BadRequest("Game not in progress");

            room.CurrentAnswers[playerId] = answer;

            if (room.CurrentAnswers.Count == room.Players.Count)
            {
                var correctAnswer = room.Questions[room.CurrentQuestionIndex].CorrectAnswer;

                foreach (var (id, ans) in room.CurrentAnswers)
                {
                    if (ans == correctAnswer)
                    {
                        if (!room.Scores.ContainsKey(id))
                            room.Scores[id] = 0;

                        room.Scores[id] += 1;
                    }
                }

                room.CurrentAnswers.Clear();
                room.CurrentQuestionIndex++;

                if (room.CurrentQuestionIndex >= room.Questions.Count)
                    room.Status = RoomStatus.Finished;
            }

            return Ok();
        }

        [HttpGet("{roomId}/results")]
        public ActionResult GetResults(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
                return NotFound();

            var question = room.Questions[Math.Max(0, room.CurrentQuestionIndex - 1)];
            return Ok(new
            {
                correct = question.CorrectAnswer,
                scores = room.Scores
            });
        }

        [HttpGet("{roomId}/final")]
        public ActionResult GetFinal(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
                return NotFound();

            if (room.Status != RoomStatus.Finished)
                return BadRequest("Game not finished");

            var leaderboard = room.Scores
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new
                {
                    playerId = kv.Key,
                    name = room.Players.First(p => p.Id == kv.Key).Name,
                    score = kv.Value
                });

            return Ok(leaderboard);
        }
    }
}
