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
                // Tried to do it with animation but it didn't work

                //Storyboard storyboard = new Storyboard();

                //DoubleAnimation animationX = new DoubleAnimation();
                //DoubleAnimation animationY = new DoubleAnimation();

                //animationX.From = position.X;
                //animationY.From = position.Y;

                currentPosition.X += position.X;
                currentPosition.Y += position.Y;

                //animationX.To = currentPosition.X;
                //animationY.To = currentPosition.Y;
                //animationX.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
                //animationY.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

                //Storyboard.SetTarget(animationX, AsRectangle);
                //Storyboard.SetTargetProperty(animationX, new PropertyPath(TranslateTransform.XProperty));

                //Storyboard.SetTarget(animationY, AsRectangle);
                //Storyboard.SetTargetProperty(animationY, new PropertyPath(TranslateTransform.XProperty));

                //storyboard.Children.Add(animationX);
                //storyboard.Children.Add(animationY);



                //storyboard.Begin();

                //AsRectangle.BeginStoryboard(storyboard);
                //AsRectangle.BeginAnimation(TranslateTransform.XProperty, animationX);

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

        public SnakeModel() : this(300, 300, 20) { }
        public SnakeModel(int startPositionX, int startPositionY, double bodyPartSize)
        {
            StartPositionX = startPositionX;
            StartPositionY = startPositionY;
            BodyPartSize = bodyPartSize;

            CreateBody();
        }

        private void CreateBody()
        {
            BodyPart tail = CreateBodyPart(StartPositionX - Convert.ToInt32(BodyPartSize) * 2, StartPositionY, BodyPartSize);
            BodyPart body = CreateBodyPart(StartPositionX - Convert.ToInt32(BodyPartSize), StartPositionY, BodyPartSize);
            BodyPart head = CreateBodyPart(StartPositionX, StartPositionY, BodyPartSize);

            BodyParts.Add(tail);
            BodyParts.Add(body);
            BodyParts.Add(head);

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
            }

            bodyPart.UpdatePosition(new Point(0, -BodyPartSize));
        }
        private void MoveDown(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.Y + BodyPartSize >= 500)
            {
                IsAlive = false;
            }

            bodyPart.UpdatePosition(new Point(0, BodyPartSize));
        }
        private void MoveLeft(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X < BodyPartSize)
            {
                IsAlive = false;
            }

            bodyPart.UpdatePosition(new Point(-BodyPartSize, 0));
        }
        private void MoveRight(BodyPart bodyPart)
        {
            if (bodyPart.CurrentPosition.X + BodyPartSize >= 500)
            {
                IsAlive = false;
            }

            bodyPart.UpdatePosition(new Point(BodyPartSize, 0));
        }

        private BodyPart CreateBodyPart(int x, int y, double size)
        {
            BodyPart bodyPart = new BodyPart();

            bodyPart.Size = size;
            bodyPart.StartPosition = new Point(x, y);
            bodyPart.Color = Brushes.White;

            return bodyPart;
        }
        public void ExtendBody()
        {
            BodyPart newPart = new BodyPart();

            newPart.Color = Brushes.White;
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
            }
        }
    }
}
