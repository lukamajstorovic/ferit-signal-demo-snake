using Ferit.SignalR.Demo.Enums;
using Ferit.SignalR.Demo.Models.Ferit.SignalR.Demo.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace Ferit.SignalR.Demo.Models
{
    public class RoomsContainer
    {
        private Dictionary<string, GameRoom> Rooms { get; set; }
        private List<string> RoomNames { get { return Rooms.Keys.ToList(); } }

        public GameRoom this[string name]
        {
            get
            {
                return Rooms[name];
            }
        }

        public RoomsContainer()
        {
            Rooms = new();
        }

        public GameRoom CreateRoom(string name, int maxPlayers)
        {
            CheckRoomName(name);
            Rooms.Add(name, new GameRoom(maxPlayers));

            return Rooms[name];
        }

        public void CheckRoomName(string name)
        {
            if (Rooms.ContainsKey(name))
            {
                throw new HubException("Room name is already in use.");
            }
        }

        public void RemoveRooms()
        {
            RoomNames.ForEach(r =>
            {
                if (!Rooms[r].Players.Any())
                {
                    Rooms.Remove(r);
                }
            });
        }

        public void OnPlayerLeft(string connectionId)
        {
            RoomNames.ForEach(r =>
            {
                Rooms[r].RemovePlayer(connectionId);
            });
        }

        public IEnumerable<RoomDetails> GetAllRoomsStatus()
        {
            return RoomNames.Select(r => new RoomDetails(Rooms[r], r));
        }

        public void CheckUsernameAndColor(string room, string user, int color)
        {
            if (Rooms[room].Players.Any(p => p.Name == user))
            {
                throw new HubException("Username is already in use");
            }
            if (Rooms[room].Players.Any(p => p.Colors == (SnakeColors)color))
            {
                throw new HubException("Color is already in use");
            }
        }

        public void JoinPlayer(string connectionId, string room, string user, int color)
        {
            if (Rooms[room].Players.Count + 1 > Rooms[room].MaxPlayers)
            {
                throw new HubException("Room is full");
            }

            Rooms[room].AddPlayer(user, connectionId, color);
        }

        public void LeaveRoom(string connectionId, string room)
        {
            Rooms[room].RemovePlayer(connectionId);
            RemoveRooms();
        }

        public void OnPlayerChangedDirection(string room, string connectionId, SnakeDirection dir)
        {
            Rooms[room].ChangeSnakeDirection(connectionId, dir);
        }
    }
}
