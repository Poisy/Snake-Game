using Snake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Snake.Data;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SnakeModel Snake { get; set; }
        private FoodModel Food { get; set; }
        private bool IsGameStarted { get; set; } = false;
        private bool IsPaused { get; set; } = false;
        private DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();
        private int Score { get; set; }
        private DateTime Timer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            DispatcherTimer.Tick += MainLoop;
        }

        private void MainLoop(object sender, EventArgs e)
        {
            Snake.Move();

            if (!Snake.IsAlive)
            {
                Stop();
                return;
            }

            if (Snake.TryToEat(Food))
            {
                Snake.ExtendBody();
                Food.NewPosition();

                Score += 100;

                ScoreTextBlock.Text = Score.ToString();
            }

            Update();
        }

        private void Start(object sender, MouseButtonEventArgs e)
        {
            Start();
        }
        private void Start()
        {
            IsGameStarted = true;

            HideAreaTextBlocks();

            Score = 0;
            Timer = new DateTime();

            Snake = new SnakeModel();
            Food = new FoodModel();

            Update();

            DispatcherTimer.Start();
        }
        private void Stop()
        {
            IsGameStarted = false;

            Area.Children.Clear();
            DispatcherTimer.Stop();

            Snake = new SnakeModel();
            Food = new FoodModel();

            ShowAreaTextBlocks();
        }
        private void Pause(object sender, MouseButtonEventArgs e)
        {
            if (!IsPaused)
            {
                DispatcherTimer.Stop();
            }
            else
            {
                DispatcherTimer.Start();
            }
            
            IsPaused = !IsPaused;
        }
        private void Exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Update()
        {
            Area.Children.Clear();

            foreach (var square in Snake.GetValue())
            {
                Area.Children.Add(square);
            }
            Area.Children.Add(Food.AsRectangle);

            Timer = Timer.AddMilliseconds(100);

            TimeTextBlock.Text = Timer.Minute.ToString() + ":" + Timer.Second.ToString();
        }

        private void MoveSnakeEvent(object sender, KeyEventArgs e)
        {
            if (IsGameStarted && !IsPaused)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if (Snake.CurrentDirection != SnakeDirections.Down)
                            Snake.CurrentDirection = SnakeDirections.Up;
                        break;
                    case Key.Down:
                        if (Snake.CurrentDirection != SnakeDirections.Up)
                            Snake.CurrentDirection = SnakeDirections.Down;
                        break;
                    case Key.Left:
                        if (Snake.CurrentDirection != SnakeDirections.Right)
                            Snake.CurrentDirection = SnakeDirections.Left;
                        break;
                    case Key.Right:
                        if (Snake.CurrentDirection != SnakeDirections.Left)
                            Snake.CurrentDirection = SnakeDirections.Right;
                        break;
                    case Key.W:
                        if (Snake.CurrentDirection != SnakeDirections.Down)
                            Snake.CurrentDirection = SnakeDirections.Up;
                        break;
                    case Key.S:
                        if (Snake.CurrentDirection != SnakeDirections.Up)
                            Snake.CurrentDirection = SnakeDirections.Down;
                        break;
                    case Key.A:
                        if (Snake.CurrentDirection != SnakeDirections.Right)
                            Snake.CurrentDirection = SnakeDirections.Left;
                        break;
                    case Key.D:
                        if (Snake.CurrentDirection != SnakeDirections.Left)
                            Snake.CurrentDirection = SnakeDirections.Right;
                        break;
                }

                Update();
            }
            else
            {
                if (e.Key == Key.Space && !IsPaused)
                {
                    Start();
                }
            }
        }

        private void HideAreaTextBlocks()
        {
            StartTextBlock.Visibility = Visibility.Hidden;
            TitleTextBlock.Visibility = Visibility.Hidden;
        }
        private void ShowAreaTextBlocks()
        {
            StartTextBlock.Visibility = Visibility.Visible;
            TitleTextBlock.Visibility = Visibility.Visible;
        }
    }
}
