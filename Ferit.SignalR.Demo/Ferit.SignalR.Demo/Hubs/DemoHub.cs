using Ferit.SignalR.Demo.Enums;
using Ferit.SignalR.Demo.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferit.SignalR.Demo.Hubs
{
    public class DemoHub : Hub
    {
        private static readonly RoomsContainer rooms = new();
        private static IHubContext<DemoHub> ctx;


        public DemoHub(IHubContext<DemoHub> hubCtx)
        {
            ctx = hubCtx;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            rooms.OnPlayerLeft(Context.ConnectionId);
            await PublishRoomStatus(Clients.All);
            await base.OnDisconnectedAsync(exception);
        }

        public override async Task OnConnectedAsync()
        {
            await PublishRoomStatus(Clients.Caller);
            await base.OnConnectedAsync();
        }

        public async IAsyncEnumerable<RoomState> Streaming(string room)
        {
            while (true)
            {
                yield return rooms[room].GetState();
                await Task.Delay(rooms[room].Delay / 2);
            }
        }

        public void ChangeDirection(string room, SnakeDirection dir)
        {
            rooms.OnPlayerChangedDirection(room, Context.ConnectionId, dir);
        }

        public async Task UserJoin(string room, string user, int color)
        {
            rooms.CheckUsernameAndColor(room, user, color);

            rooms.JoinPlayer(Context.ConnectionId, room, user, color);
            await PublishRoomStatus(Clients.All);
        }

        public async Task UserLeft(string room)
        {
            rooms.LeaveRoom(Context.ConnectionId, room);

            rooms.RemoveRooms();//delete if room is empty

            await PublishRoomStatus(Clients.All);
        }

        public async Task RoomCreate(string name, int maxPlayers)
        {
            rooms.CreateRoom(name, maxPlayers);
            await PublishRoomStatus(Clients.All);
        }

        public void CheckRoomName(string name)
        {
            rooms.CheckRoomName(name);
        }

        public async Task RoomsRemove()
        {
            rooms.RemoveRooms();
            await PublishRoomStatus(Clients.All);
        }

        private static async Task PublishRoomStatus(IClientProxy clients)
        {
            await clients.SendAsync("RoomStatusResponse", rooms.GetAllRoomsStatus());
        }

        public async Task IsUserInRoom(string room)
        {
            var isConnected = rooms[room].Players.Any(p => p.ConnectionId == Context.ConnectionId);
            await Clients.Caller.SendAsync("IsUserInRoom", isConnected);
        }
    }
}
