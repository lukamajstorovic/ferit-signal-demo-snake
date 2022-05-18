using Ferit.SignalR.Demo.Enums;
using Ferit.SignalR.Demo.Models.Ferit.SignalR.Demo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ferit.SignalR.Demo.Models
{
    public class RoomDetails
    {
        public RoomDetails(GameRoom room, string roomName)
        {
            PlayersCount = room.Players.Count;
            MaxPlayers = room.MaxPlayers;
            Name = roomName;
            Usernames = room.Players.Select(p => p.Name);
            Colors = room.Players.Select(p => p.Colors);
        }

        public int PlayersCount { get; set; }
        public int MaxPlayers { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Usernames { get; set; }
        public IEnumerable<SnakeColors> Colors { get; set; }

    }
}