using Ferit.SignalR.Demo.Enums;
using Ferit.SignalR.Demo.Models.Fields;

namespace Ferit.SignalR.Demo.Models
{
    public class DrawField : Field
    {
        public DrawField(GameField field) : base(field.X, field.Y)
        {
            State = field.State;
            Direction = State == FieldState.SnakeHead ? field.Player?.Snake.Direction : null;
            Color = GetColorFromGameField(field);
        }

        public string Color { get; set; }
        public FieldState State { get; set; }
        public SnakeDirection? Direction { get; set; }


        private static string GetColorFromGameField(GameField f)
        {
            return f.State switch
            {
                FieldState.SnakeHead => GameConstants.SNAKE_HEAD_COLOR,
                FieldState.SnakeBody => (f.Player?.Colors)?.ToString(),
                FieldState.Food => GameConstants.FOOD_COLOR,
                _ => null,
            };
        }
    }

}

