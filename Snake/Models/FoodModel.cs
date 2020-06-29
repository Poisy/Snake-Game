using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake.Data
{
    public class FoodModel
    {
        public int Size { get; set; }
        public Point Position { get; set; }
        public Rectangle AsRectangle { get; set; }
        private static Random Random { get; set; } = new Random();

        public FoodModel() : this(20) { }
        public FoodModel(int size)
        {
            Size = size;

            NewPosition();
        }

        public void NewPosition()
        {
            AsRectangle = new Rectangle();

            AsRectangle.Fill = Settings.FoodColor;
            AsRectangle.Width = Size;
            AsRectangle.Height = Size;

            Position = new Point(Random.Next(0, 500 / Size) * Size, Random.Next(0, 500 / Size) * Size);

            Canvas.SetLeft(AsRectangle, Position.X);
            Canvas.SetTop(AsRectangle, Position.Y);
        }
    }
}
