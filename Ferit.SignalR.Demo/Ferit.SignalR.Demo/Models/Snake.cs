using Ferit.SignalR.Demo.Enums;
using Ferit.SignalR.Demo.Models.Fields;
using System;
using System.Collections.Generic;

namespace Ferit.SignalR.Demo.Models
{
    public class Snake
    {
        public List<Field> Parts { get; private set; }
        public SnakeDirection Direction { get; private set; }
        public int Points { get; private set; }

        public int Size
        {
            get
            {
                return Parts.Count;
            }
        }

        public Field Head
        {
            get
            {
                return Parts[0];
            }
        }

        public Field BodyBeginning
        {
            get
            {
                return Parts[1];
            }
        }

        public Snake(GameField[][] fields)
        {
            Direction = SnakeDirection.Up;
            Parts = GetSnakeFields(fields);
        }


        public static List<Field> GetSnakeFields(GameField[][] fields)
        {
            var random = new Random();
            var x = random.Next(GameConstants.FIELD_SIZE);
            var y = random.Next(GameConstants.FIELD_SIZE);

            while (fields[x][y].State != FieldState.Empty &&
                   fields[SetNextCoordinateValue(x + 1)][y].State != FieldState.Empty &&
                   fields[SetNextCoordinateValue(x + 2)][y].State != FieldState.Empty)
            {
                x = random.Next(GameConstants.FIELD_SIZE);
                y = random.Next(GameConstants.FIELD_SIZE);
            }

            return new List<Field>
            {
                new Field(x, y),
                new Field(x + 1, y),
                new Field(x + 2, y)
            };

        }

        private static int SetNextCoordinateValue(int coordinate)
        {
            if (coordinate > GameConstants.FIELD_SIZE - 1)
            {
                return 0;
            }
            else if (coordinate < 0)
            {
                return GameConstants.FIELD_SIZE - 1;
            }
            else
            {
                return coordinate;
            }
        }

        private Field GetNextField(SnakeDirection? direction = null)
        {
            var nextPos = new Field(Head.X, Head.Y);

            switch (direction ?? Direction)
            {
                case SnakeDirection.Up:
                    nextPos.Y -= 1; break;
                case SnakeDirection.Down:
                    nextPos.Y += 1; break;
                case SnakeDirection.Left:
                    nextPos.X -= 1; break;
                case SnakeDirection.Right:
                    nextPos.X += 1; break;
            }


            nextPos.X = SetNextCoordinateValue(nextPos.X);
            nextPos.Y = SetNextCoordinateValue(nextPos.Y);

            return nextPos;
        }

        public void Move()
        {
            var next = GetNextField();

            for (var i = Parts.Count - 1; i > 0; i--)
            {
                Parts[i].X = Parts[i - 1].X;
                Parts[i].Y = Parts[i - 1].Y;
            }

            Parts[0] = next;

        }

        public void TrySetDirection(SnakeDirection direction)
        {
            if (direction == Direction)
            {
                return;
            }

            var nextField = GetNextField(direction);

            if (!BodyBeginning.IsSameCoordinate(nextField))
            {
                Direction = direction;
            }
        }

    }


}
