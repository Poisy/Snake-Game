using System;
using System.Collections.Generic;
using System.Windows.Media;
using Snake.Data;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

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
        public int AreaWidth { get; set; }
        public int AreaHight { get; set; }
        public double BodyPartSize { get; set; }
        public int CountBodyParts { get { return BodyParts.Count; } }
        public bool IsAlive { get; set; } = true;
        public Brush HeadColor { get; private set; }
        public Brush BodyColor { get; private set; }
        public string ReasonDied { get; private set; } = "Error";
        public bool CanTeleport { get; set; } = true;
        public short Starve
        {
            get
            {
                return starve;
            }
            set
            {
                if (starve + value > 60) starve = 60;
                else starve += value;
            }
        }

        private short starve;

        public SnakeModel() : this(500, 500, 20, Brushes.White, Brushes.White) { }
        public SnakeModel(Brush bodyColor, Brush headColor) : this(500, 500, 20, bodyColor, headColor) { }
        public SnakeModel(int areaWidth, int areaHeight, double bodyPartSize, Brush bodyColor, Brush headColor)
        {
            AreaWidth = areaWidth;
            AreaHight = areaHeight;
            BodyPartSize = bodyPartSize;

            HeadColor = headColor;
            BodyColor = bodyColor;

            CreateBody();
        }

        private void CreateBody()
        {
            BodyPart head = CreateBodyPart(AreaWidth/2+(int)(AreaWidth/2%BodyPartSize) - Convert.ToInt32(BodyPartSize) * 2, AreaWidth / 2 + (int)(AreaWidth/2 % BodyPartSize), BodyPartSize, HeadColor);
            BodyPart body = CreateBodyPart(AreaWidth/2 + (int)(AreaWidth/2 % BodyPartSize) - Convert.ToInt32(BodyPartSize), AreaWidth / 2 + (int)(AreaWidth/2 % BodyPartSize), BodyPartSize, BodyColor);
            BodyPart tail = CreateBodyPart(AreaWidth/2 + (int)(AreaWidth/2 % BodyPartSize), AreaWidth / 2 + (int)(AreaWidth/2 % BodyPartSize), BodyPartSize, BodyColor);

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
            if (Settings.Difficulty == Difficulties.Python)
            {
                if (Starve <= 0)
                {
                    IsAlive = false;
                    ReasonDied = DyingReasons.Starving;

                    return;
                }

                Starve = -1;
            }

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
                if (CanTeleport)
                {
                    bodyPart.UpdatePosition(new Point(0, AreaHight - BodyPartSize));
                    return;
                }

                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(0, -BodyPartSize));
        }
        private void MoveDown(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.Y + BodyPartSize >= AreaHight)
            {
                if (CanTeleport)
                {
                    bodyPart.UpdatePosition(new Point(0, -(AreaHight - BodyPartSize)));
                    return;
                }

                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(0, BodyPartSize));
        }
        private void MoveLeft(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X < BodyPartSize)
            {
                if (CanTeleport)
                {
                    bodyPart.UpdatePosition(new Point(AreaHight - BodyPartSize, 0));
                    return;
                }

                IsAlive = false;
                ReasonDied = DyingReasons.HitTheWall;
            }

            bodyPart.UpdatePosition(new Point(-BodyPartSize, 0));
        }
        private void MoveRight(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X + BodyPartSize >= AreaWidth)
            {
                if (CanTeleport)
                {
                    bodyPart.UpdatePosition(new Point(-(AreaHight - BodyPartSize), 0));
                    return;
                }

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

        public bool TryToEat(Rectangle rect)
        {
            Point rectP = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
            Point headP = BodyParts[0].CurrentPosition;

            if (rectP.X == headP.X && rectP.Y == headP.Y)
            {
                Starve = 60;

                return true;
            }

            return false;
        }

        public void DidHitAWall(Rectangle rect)
        {
            Point rectP = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
            Point headP = BodyParts[0].CurrentPosition;

            if (headP.X >= rectP.X && headP.X < rectP.X + rect.Width)
            {
                if (headP.Y >= rectP.Y && headP.Y < rectP.Y + rect.Height)
                {
                    IsAlive = false;
                    ReasonDied = DyingReasons.HitTheWall;
                }
            }
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
