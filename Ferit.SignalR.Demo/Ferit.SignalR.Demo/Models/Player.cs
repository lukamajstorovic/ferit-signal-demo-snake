using Ferit.SignalR.Demo.Enums;

namespace Ferit.SignalR.Demo.Models
{
    public class Player
    {
        public SnakeColors Colors { get; set; }
        public PlayerGameState State { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public Snake Snake { get; set; }
    }
}
