namespace Ferit.SignalR.Demo.Models.Fields
{
    public class Field
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Field(int x, int y)
        {
            X = x;
            Y = y;
        }
        public bool IsSameCoordinate(Field compareField)
        {
            return X == compareField.X && Y == compareField.Y;
        }
    }
}
