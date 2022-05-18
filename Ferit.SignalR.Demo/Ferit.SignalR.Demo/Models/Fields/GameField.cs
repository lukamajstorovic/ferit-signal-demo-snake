using Ferit.SignalR.Demo.Enums;

namespace Ferit.SignalR.Demo.Models.Fields
{
    public class GameField : Field
    {
        public Player Player { get; set; }
        public FieldState State { get; set; }
        public GameField(int x, int y, FieldState state) : base(x, y)
        {
            State = state;
        }
    }
}
