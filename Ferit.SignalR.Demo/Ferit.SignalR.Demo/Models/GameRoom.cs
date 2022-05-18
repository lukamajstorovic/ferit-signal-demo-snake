namespace Ferit.SignalR.Demo.Models
{

    using global::Ferit.SignalR.Demo.Enums;
    using global::Ferit.SignalR.Demo.Models.Fields;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    namespace Ferit.SignalR.Demo.Models
    {
        public class GameRoom
        {
            public List<Player> Players = new();
            public FoodState Food = new();
            public object obj = new();
            private readonly Timer _timer;
            public int MaxPlayers { get; private set; }
            public GameField[][] Fields { get; set; }
            public int Delay { get; private set; } = GameConstants.MOVE_SPEED_LVL1;


            public GameRoom(int maxPlayers)
            {
                Fields = new GameField[GameConstants.FIELD_SIZE][].Select(t => new GameField[GameConstants.FIELD_SIZE].ToArray()).ToArray();
                for (var x = 0; x < Fields.Length; x++)
                {
                    for (var y = 0; y < Fields[x].Length; y++)
                    {
                        Fields[x][y] = new GameField(x, y, FieldState.Empty);
                    }
                }

                MaxPlayers = maxPlayers;
                _timer = new Timer((t) => Move(), null, 0, Delay);
            }


            public void AddPlayer(string username, string connectionId, int color)
            {
                var id = Players.FindIndex(v => string.Equals(v.Name, username));

                if (id > -1)
                {
                    Players.RemoveAt(id);
                }

                Players.Add(new Player
                {
                    Name = username,
                    ConnectionId = connectionId,
                    Colors = (SnakeColors)color,
                    Snake = new Snake(Fields),
                    State = PlayerGameState.Running
                });

                Food.CreateFood(Fields);
            }

            public void RemovePlayer(string connectionId)
            {
                var id = Players.FindIndex(v => string.Equals(v.ConnectionId, connectionId));

                if (id > -1)
                {
                    Players.RemoveAt(id);
                }
            }


            private void RemovePendingPlayers()
            {
                var deletePlayers = Players.Where(p => p.State == PlayerGameState.PendingRemove).ToList();
                if (deletePlayers.Any())
                {
                    deletePlayers.ForEach(d => RemovePlayer(d.ConnectionId));
                }
            }




            private void UpdateSpeed()
            {
                if (Food.FoodEaten >= GameConstants.LVL2_THRESHOLD &&
                   Food.FoodEaten < GameConstants.LVL3_THRESHOLD)
                {
                    Delay = GameConstants.MOVE_SPEED_LVL2;
                    _timer.Change(0, Delay);
                }
                else if (Food.FoodEaten >= GameConstants.LVL3_THRESHOLD)
                {
                    Delay = GameConstants.MOVE_SPEED_LVL3;
                    _timer.Change(0, Delay);
                }
            }

            public IEnumerable<GameField> FilterFields(Func<GameField, bool> func)
            {
                return Fields.SelectMany(row => row.Where(func));
            }

            public void ClearFields()
            {
                FilterFields(v => v.State == FieldState.SnakeHead || v.State == FieldState.SnakeBody)
                    .ToList().ForEach(f =>
                    {
                        Fields[f.X][f.Y].State = FieldState.Empty;
                        Fields[f.X][f.Y].Player = null;
                    });
            }

            public void ChangeSnakeDirection(string connectionId, SnakeDirection dir)
            {
                Players.First(p => p.ConnectionId == connectionId).Snake.TrySetDirection(dir);
            }

            public void UpdateFileds()
            {
                Players.ForEach(p =>
                {
                    for (int i = 0; i < p.Snake.Size; i++)
                    {
                        var part = p.Snake.Parts[i];
                        Fields[part.X][part.Y].State = i == 0 ? FieldState.SnakeHead : FieldState.SnakeBody;
                        Fields[part.X][part.Y].Player = p;
                    }
                });

                Food.Fields.ForEach(f => Fields[f.X][f.Y].State = FieldState.Food);
            }

            public RoomState GetState()
            {
                try
                {
                    lock (obj)
                    {
                        return new RoomState
                        {
                            PlayerStates = Players.Select(i => new PlayerState(i))
                                                  .OrderByDescending(v => v.Score).ToList(),

                            DrawFields = FilterFields(r => r.State != FieldState.Empty)
                                                  .Select(col => new DrawField(col)).ToList()
                        };
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    return null;
                }
            }

            private void Move()
            {
                try
                {
                    lock (obj)
                    {
                        Players.ForEach(p => p.Snake.Move());
                        //CheckCollisions();
                        ClearFields();
                        UpdateFileds();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                }
            }
        }
    }

}

