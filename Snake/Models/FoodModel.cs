using Snake.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake.Data
{
    public class FoodModel
    {
        public FoodType Type { get; private set; }
        public int Size { get; set; }
        public Point Position { get; set; }
        public Rectangle AsRectangle { get; set; }
        public static bool IsGoldenFoodShown { get; set; } = false;
        public static bool IsSpecialFoodShown { get; set; } = false;
        public static List<ObstacleModel> Walls { get; set; }
        private static Random Random { get; set; } = new Random();

        public FoodModel() : this(20) { }
        public FoodModel(int size)
        {
            Size = size;
            Type = FoodType.Normal;
        }

        public void NewPosition(SnakeModel snake)
        {
            AsRectangle = new Rectangle();

            switch (Type)
            {
                case FoodType.Normal:
                    AsRectangle.Fill = Settings.FoodColor;
                    break;
                case FoodType.Gold:
                    AsRectangle.Fill = Brushes.Gold;
                    break;
                case FoodType.Special:
                    AsRectangle.Fill = Brushes.Pink;
                    break;
            }

            AsRectangle.Width = Size;
            AsRectangle.Height = Size;

            while (true)
            {
                Position = new Point(Random.Next(0, 500 / Size) * Size, Random.Next(0, 500 / Size) * Size);

                if (IsInsideSnakeBody(snake)) continue;

                if (MainWindow.CanShowWalls)
                {
                    if (IsInsideWalls()) continue;
                }

                break;
            }

            Canvas.SetLeft(AsRectangle, Position.X);
            Canvas.SetTop(AsRectangle, Position.Y);
        }
        private bool IsInsideSnakeBody(SnakeModel snake)
        {
            foreach (var bodyPart in snake.BodyParts)
            {
                Point pos = new Point(bodyPart.CurrentPosition.X, bodyPart.CurrentPosition.Y);

                if (pos.X == Position.X && pos.Y == Position.Y) return true;
            }

            return false;
        }
        private bool IsInsideWalls()
        {
            foreach (var wall in Walls)
            {
                if (Position.X >= wall.Position.X && Position.X <= wall.Position.X + wall.Width)
                {
                    if (Position.Y >= wall.Position.Y && Position.Y <= wall.Position.Y + wall.Height) return true;
                }
            }

            return false;
        }

        public static void ChanceOtherFoodToAppear(List<FoodModel> foods, SnakeModel snake)
        {
            if (Random.Next(0, 20) == 10 && !IsGoldenFoodShown)
            {
                foods.Add(CreateGoldenFood(snake));
            }
            else if (Random.Next(0, 4) == 2 && !IsSpecialFoodShown)
            {
                foods.Add(CreateSpecialFood(snake));
            }
        }

        private static FoodModel CreateGoldenFood(SnakeModel snake)
        {
            FoodModel food = new FoodModel();

            food.Type = FoodType.Gold;
            food.NewPosition(snake);

            IsGoldenFoodShown = true;

            return food;
        }
        private static FoodModel CreateSpecialFood(SnakeModel snake)
        {
            FoodModel food = new FoodModel();

            food.Type = FoodType.Special;
            food.NewPosition(snake);

            IsSpecialFoodShown = true;

            return food;
        }

        public static void ChangeFoodColor(List<FoodModel> foods, Brush brush)
        {
            foreach (var food in foods)
            {
                if (food.AsRectangle.Fill == Settings.FoodColor)
                {
                    food.AsRectangle.Fill = brush;
                }
            }

            Settings.FoodColor = brush;
        }
    }
}
