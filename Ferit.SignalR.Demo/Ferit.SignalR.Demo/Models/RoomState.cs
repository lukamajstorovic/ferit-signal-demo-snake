using Ferit.SignalR.Demo.Models.Fields;
using System.Collections.Generic;

namespace Ferit.SignalR.Demo.Models
{
    public class RoomState
    {
        public List<PlayerState> PlayerStates { get; set; }
        public List<DrawField> DrawFields { get; set; }
    }
}
