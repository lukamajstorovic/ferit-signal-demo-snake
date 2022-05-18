using Ferit.SignalR.Demo.Models.Fields;
using System;
using System.Collections.Generic;

namespace Ferit.SignalR.Demo.Models
{
    public class FoodState
    {
        public List<Field> Fields = new();
        public int FoodEaten { get; private set; }
        public bool ShouldAddFood()
        {
            return Fields.Count < GameConstants.FOOD_MAX_COUNT;
        }
        private static Field GetFreeField(GameField[][] field)
        {
            var random = new Random();
            var x = random.Next(GameConstants.FIELD_SIZE);
            var y = random.Next(GameConstants.FIELD_SIZE);
            while (field[x][y].State != Enums.FieldState.Empty)
            {
                x = random.Next(GameConstants.FIELD_SIZE);
                y = random.Next(GameConstants.FIELD_SIZE);
            }
            return new Field(x, y);
        }
        public void CreateFood(GameField[][] field)
        {
            while (ShouldAddFood())
            {
                Fields.Add(GetFreeField(field));
            }
        }
    }
}
