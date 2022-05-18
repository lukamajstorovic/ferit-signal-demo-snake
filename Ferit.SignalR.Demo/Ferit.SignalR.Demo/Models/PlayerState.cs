using Ferit.SignalR.Demo.Enums;

namespace Ferit.SignalR.Demo.Models
{
    public class PlayerState
    {
        public PlayerState(Player player)
        {
            Name = player.Name;
            Score = player.Snake.Points;
            Color = player.Colors;
        }

        public string Name { get; set; }
        public int Score { get; set; }
        public SnakeColors Color { get; set; }
    }
}
