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
using Snake.Models;
using Snake.Windows;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SnakeModel Snake { get; set; }
        public List<FoodModel> Food { get; set; } = new List<FoodModel>();
        private bool IsGameStarted { get; set; } = false;
        private bool IsPaused { get; set; } = false;
        public DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();
        private int Score { get; set; }
        private DateTime Timer { get; set; }
        private string TimerToString => (Timer.Minute > 9 ? Timer.Minute.ToString() : "0" + Timer.Minute.ToString()) +
                ":" + (Timer.Second > 9 ? Timer.Second.ToString() : "0" + Timer.Second.ToString());

        public MainWindow()
        {
            InitializeComponent();

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

            foreach(var food in Food)
            {
                if (Snake.TryToEat(food))
                {
                    Snake.ExtendBody();
                    food.NewPosition();

                    Score += 100;

                    _scoreTextBlock.Text = Score.ToString();

                    for (int i = 0; i < Settings.FoodSpawnRate - 1; i++)
                    {
                        Food.Add(new FoodModel());
                    }

                    break;
                }
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

            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);

            _scoreTextBlock.Text = "0";
            _timeTextBlock.Text = "00:00";
            _area.Background = Settings.Background;

            HideAreaTextBlocks();

            Score = 0;
            Timer = new DateTime();

            Snake = new SnakeModel(Settings.BodyColor, Settings.HeadColor);

            for (int i = Settings.FoodSpawnCount; i > 0; i--)
            {
                Food.Add(new FoodModel());
            }

            _pauseTextBlock.Text = "\uf04c";

            Update();

            DispatcherTimer.Start();
        }
        private void Stop()
        {
            IsGameStarted = false;

            _reasonDiedTextBlock.Text = Snake.ReasonDied;
            _scoreResultTextBlock.Text = $"Time: {TimerToString}       Score: {Score}";

            _area.Children.Clear();
            DispatcherTimer.Stop();

            Snake = new SnakeModel();
            Food = new List<FoodModel>();

            _pauseTextBlock.Text = "\uf04b";

            

            ShowAreaTextBlocks();
        }
        private void Pause(object sender, MouseButtonEventArgs e)
        {
            if (IsGameStarted) Pause();
            else Start();
        }
        private void Pause()
        {
            if (!IsPaused)
            {
                DispatcherTimer.Stop();
                _pauseTextBlock.Text = "\uf04b";
            }
            else
            {
                DispatcherTimer.Start();
                _pauseTextBlock.Text = "\uf04c";
            }

            IsPaused = !IsPaused;
        }
        private void Exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Update()
        {
            _area.Children.Clear();

            foreach (var square in Snake.GetValue())
            {
                _area.Children.Add(square);
            }

            foreach (var food in Food)
            {
                _area.Children.Add(food.AsRectangle);
            }


            Timer = Timer.AddMilliseconds(100);

            _timeTextBlock.Text = TimerToString;
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
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
                    case Key.Space:
                        Pause();
                        break;
                }

                Update();
            }
            else
            {
                if (e.Key == Key.Space)
                {
                    if (!IsGameStarted) Start();
                    else Pause();   
                }
            }
        }

        private void HideAreaTextBlocks()
        {
            _startTextBlock.Visibility = Visibility.Hidden;
            _titleTextBlock.Visibility = Visibility.Hidden;
            _restartTextBlock.Visibility = Visibility.Hidden;
            _scoreResultTextBlock.Visibility = Visibility.Hidden;
            _reasonDiedTextBlock.Visibility = Visibility.Hidden;
        }
        private void ShowAreaTextBlocks()
        {
            _titleTextBlock.Visibility = Visibility.Visible;
            _restartTextBlock.Visibility = Visibility.Visible;
            _scoreResultTextBlock.Visibility = Visibility.Visible;
            _reasonDiedTextBlock.Visibility = Visibility.Visible;
        }

        private void OpenDeveloperWindow(object sender, MouseButtonEventArgs e)
        {
            if (IsPaused || !IsGameStarted) new DeveloperWindow().Show();
        }
    }
}
