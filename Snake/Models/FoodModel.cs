using Snake.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Snake.Data
{
    public class FoodModel
    {
        public int Size { get; set; }
        public Point Position { get; set; }
        public Rectangle AsRectangle { get; set; }
        public static List<ObstacleModel> Walls { get; set; }
        private static Random Random { get; set; } = new Random();

        public FoodModel() : this(20) { }
        public FoodModel(int size)
        {
            Size = size;
        }

        public void NewPosition(SnakeModel snake)
        {
            AsRectangle = new Rectangle();

            AsRectangle.Fill = Settings.FoodColor;
            AsRectangle.Width = Size;
            AsRectangle.Height = Size;

            while (true)
            {
                Position = new Point(Random.Next(0, 500 / Size) * Size, Random.Next(0, 500 / Size) * Size);

                foreach (var bodyPart in snake.GetValue())
                {
                    Point pos = new Point(Canvas.GetLeft(bodyPart), Canvas.GetTop(bodyPart));

                    if (pos.X == Position.X && pos.Y == Position.Y) continue;
                }

                if (MainWindow.CanShowWalls)
                {
                    foreach (var wall in Walls)
                    {
                        if (Position.X >= wall.Position.X && Position.X < wall.Position.X + wall.Width)
                        {
                            if (Position.Y >= wall.Position.Y && Position.Y < wall.Position.Y + wall.Height) continue;
                        }
                    }
                }

                break;
            }

            Canvas.SetLeft(AsRectangle, Position.X);
            Canvas.SetTop(AsRectangle, Position.Y);
        }
    }
}
