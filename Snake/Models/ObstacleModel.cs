using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake.Models
{
    public class ObstacleModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Point Position { get; set; }
        public Rectangle AsRectangle { get; private set; }

        public ObstacleModel(int width, int height, Point position)
        {
            Width = width;
            Height = height;
            Position = position;

            CreateRectangle();
        }

        private void CreateRectangle()
        {
            AsRectangle = new Rectangle();

            AsRectangle.Fill = Brushes.Gray;
            AsRectangle.Width = Width;
            AsRectangle.Height = Height;

            Canvas.SetLeft(AsRectangle, Position.X);
            Canvas.SetTop(AsRectangle, Position.Y);
        }
    }
}
