using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Snake.Data;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Snake.Models
{
    public class SnakeModel
    {
        class BodyPart
        {
            private Point currentPosition = new Point(0, 0);
            public Point CurrentPosition 
            {
                get
                {
                    return new Point(StartPosition.X + currentPosition.X, StartPosition.Y + currentPosition.Y);
                }
            }
            public Rectangle AsRectangle { get; } = new Rectangle();
            public double Size
            {
                get
                {
                    return AsRectangle.Height;
                }
                set
                {
                    AsRectangle.Width = value;
                    AsRectangle.Height = value;
                }
            }
            public Point StartPosition 
            {
                get
                {
                    Point position = new Point();
                    position.X = Canvas.GetLeft(AsRectangle);
                    position.Y = Canvas.GetTop(AsRectangle);
                    return position;
                }
                set
                {
                    Canvas.SetLeft(AsRectangle, value.X);
                    Canvas.SetTop(AsRectangle, value.Y);
                }
            }
            public Brush Color
            {
                get
                {
                    return AsRectangle.Fill;
                }
                set
                {
                    AsRectangle.Fill = value;
                }
            }

            public void UpdatePosition(Point position)
            {
                currentPosition.X += position.X;
                currentPosition.Y += position.Y;

                AsRectangle.RenderTransform = new TranslateTransform(currentPosition.X, currentPosition.Y);
            }
        }

        private LinkedList<SnakeDirections> Directions { get; set; } = new LinkedList<SnakeDirections>();
        private List<BodyPart> BodyParts { get; set; } = new List<BodyPart>();
        public SnakeDirections CurrentDirection { get; set; }
        public int StartPositionX { get; set; }
        public int StartPositionY { get; set; }
        public double BodyPartSize { get; set; }
        public int CountBodyParts { get { return BodyParts.Count; } }
        public bool IsAlive { get; set; } = true;
        public Brush HeadColor { get; private set; }
        public Brush BodyColor { get; private set; }
        public string ReasonDied { get; private set; } = "Error";

        public SnakeModel() : this(300, 300, 20, Brushes.White, Brushes.White) { }
        public SnakeModel(Brush bodyColor, Brush headColor) : this(300, 300, 20, bodyColor, headColor) { }
        public SnakeModel(int startPositionX, int startPositionY, double bodyPartSize, Brush bodyColor, Brush headColor)
        {
            StartPositionX = startPositionX;
            StartPositionY = startPositionY;
            BodyPartSize = bodyPartSize;

            HeadColor = headColor;
            BodyColor = bodyColor;

            CreateBody();
        }

        private void CreateBody()
        {
            BodyPart head = CreateBodyPart(StartPositionX - Convert.ToInt32(BodyPartSize) * 2, StartPositionY, BodyPartSize, HeadColor);
            BodyPart body = CreateBodyPart(StartPositionX - Convert.ToInt32(BodyPartSize), StartPositionY, BodyPartSize, BodyColor);
            BodyPart tail = CreateBodyPart(StartPositionX, StartPositionY, BodyPartSize, BodyColor);

            BodyParts.Add(head);
            BodyParts.Add(body);
            BodyParts.Add(tail);

            Directions.AddFirst(SnakeDirections.Left);
            Directions.AddFirst(SnakeDirections.Left);
            Directions.AddFirst(SnakeDirections.Left);

            CurrentDirection = SnakeDirections.Left;
        }

        public List<Rectangle> GetValue()
        {
            List<Rectangle> bodies = new List<Rectangle>();

            foreach (BodyPart part in BodyParts)
            {
                bodies.Add(part.AsRectangle);
            }

            return bodies;
        }

        public void Move()
        {
            Directions.RemoveLast();

            Directions.AddFirst(CurrentDirection);

            int i = 0;

            foreach (var direction in Directions)
            {
                switch (direction)
                {
                    case SnakeDirections.Up:
                        MoveUp(BodyParts[i]);
                        if (i != 0) DidBiteItSelf(BodyParts[i]);
                        break;
                    case SnakeDirections.Down:
                        MoveDown(BodyParts[i]);
                        if (i != 0) DidBiteItSelf(BodyParts[i]);
                        break;
                    case SnakeDirections.Left:
                        MoveLeft(BodyParts[i]);
                        if (i != 0) DidBiteItSelf(BodyParts[i]);
                        break;
                    case SnakeDirections.Right:
                        MoveRight(BodyParts[i]);
                        if (i != 0) DidBiteItSelf(BodyParts[i]);
                        break;
                }
                i++;
            }
        }

        private void MoveUp(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.Y < BodyPartSize) 
            {
                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(0, -BodyPartSize));
        }
        private void MoveDown(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.Y + BodyPartSize >= 500)
            {
                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(0, BodyPartSize));
        }
        private void MoveLeft(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X < BodyPartSize)
            {
                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(-BodyPartSize, 0));
        }
        private void MoveRight(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X + BodyPartSize >= 500)
            {
                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(BodyPartSize, 0));
        }

        private BodyPart CreateBodyPart(int x, int y, double size, Brush color)
        {
            BodyPart bodyPart = new BodyPart();

            bodyPart.Size = size;
            bodyPart.StartPosition = new Point(x, y);
            bodyPart.Color = color;

            return bodyPart;
        }
        public void ExtendBody()
        {
            BodyPart newPart = new BodyPart();

            BodyParts[0].Color = BodyColor;

            newPart.Color = HeadColor;
            newPart.Size = BodyPartSize;

            switch (CurrentDirection)
            {
                case SnakeDirections.Up:
                    newPart.StartPosition = new Point(
                        BodyParts[0].CurrentPosition.X, 
                        BodyParts[0].CurrentPosition.Y - BodyPartSize);
                    break;
                case SnakeDirections.Down:
                    newPart.StartPosition = new Point(
                        BodyParts[0].CurrentPosition.X, 
                        BodyParts[0].CurrentPosition.Y + BodyPartSize);
                    break;
                case SnakeDirections.Left:
                    newPart.StartPosition = new Point(
                        BodyParts[0].CurrentPosition.X - BodyPartSize, 
                        BodyParts[0].CurrentPosition.Y);
                    break;
                case SnakeDirections.Right:
                    newPart.StartPosition = new Point(
                        BodyParts[0].CurrentPosition.X + BodyPartSize, 
                        BodyParts[0].CurrentPosition.Y);
                    break;
            }

            BodyParts.Insert(0, newPart);
            Directions.AddFirst(CurrentDirection);
        }

        public bool TryToEat(FoodModel food)
        {
            return food.Position == BodyParts[0].CurrentPosition;
        }

        private void DidBiteItSelf(BodyPart part)
        {
            if (BodyParts[0].CurrentPosition == part.CurrentPosition)
            {
                IsAlive = false;
                ReasonDied = DyingReasons.BiteYourSelf;
            }
        }
    }
}
