﻿using System;
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
        public List<ObstacleModel> Obstacles { get; set; } = new List<ObstacleModel>();
        public DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();
        public static bool CanShowWalls { get; set; } = false;
        private bool IsGameStarted { get; set; } = false;
        private bool IsPaused { get; set; } = false;
        private List<RecordModel> Records { get; set; } = FileManager.ReadFromRecords();
        private bool DidAlreadySnakeMoved { get; set; }
        private int Score { get; set; }
        private DateTime Timer { get; set; }
        private string TimerToString => (Timer.Minute > 9 ? Timer.Minute.ToString() : "0" + Timer.Minute.ToString()) +
                ":" + (Timer.Second > 9 ? Timer.Second.ToString() : "0" + Timer.Second.ToString());

        public MainWindow()
        {
            InitializeComponent();

            DisplayRecords();

            DispatcherTimer.Tick += MainLoop;
        }

        private void MainLoop(object sender, EventArgs e)
        {
            DidAlreadySnakeMoved = false;

            Snake.Move();

            if (CanShowWalls)
            {
                foreach (var obstacle in Obstacles)
                {
                    Snake.DidHitAWall(obstacle.AsRectangle);
                }
            }

            if (!Snake.IsAlive)
            {
                Stop();
                return;
            }

            foreach(var food in Food)
            {
                if (Snake.TryToEat(food.AsRectangle))
                {
                    Snake.ExtendBody();
                    food.NewPosition(Snake);

                    Score += 100;

                    _scoreTextBlock.Text = Score.ToString();

                    for (int i = 0; i < Settings.FoodSpawnRate - 1; i++)
                    {
                        FoodModel newFood = new FoodModel();
                        newFood.NewPosition(Snake);

                        Food.Add(newFood);
                    }

                    break;
                }
            }
            

            Update();
        }

        private void Start(object sender, MouseButtonEventArgs e)
        {
            if (IsGameStarted) Restart();
            else Start();
        }
        private void Start()
        {
            IsGameStarted = true;

            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);

            _scoreTextBlock.Text = "0";
            _timeTextBlock.Text = "00:00";
            _area.Background = Settings.Background;
            _area.Opacity = 1;

            HideAreaTextBlocks();

            Score = 0;
            Timer = new DateTime();

            Snake = new SnakeModel(Settings.BodyColor, Settings.HeadColor);

            switch (Settings.Difficulty)
            {
                case Difficulties.Easy:
                    Snake.CanTeleport = true;
                    CanShowWalls = false;
                    break;
                case Difficulties.Normal:
                    Snake.CanTeleport = false;
                    CanShowWalls = false;
                    break;
                case Difficulties.Hard:
                    Snake.CanTeleport = false;
                    CanShowWalls = true;
                    CreateWalls();
                    FoodModel.Walls = Obstacles;
                    break;
                case Difficulties.Python:
                    Snake.CanTeleport = false;
                    CanShowWalls = true;
                    CreateWalls();
                    FoodModel.Walls = Obstacles;
                    Snake.Starve = 60;
                    break;
            }

            for (int i = Settings.FoodSpawnCount; i > 0; i--)
            {
                FoodModel food = new FoodModel();
                food.NewPosition(Snake);

                Food.Add(food);
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

            CheckResult();

            DisplayRecords();
        }
        private void Restart()
        {
            IsGameStarted = false;

            _area.Children.Clear();
            DispatcherTimer.Stop();

            Snake = new SnakeModel();
            Food = new List<FoodModel>();

            _pauseTextBlock.Text = "\uf04b";

            _pauseTextTextBlock.Visibility = Visibility.Hidden;
            _restartTextBlock.Visibility = Visibility.Hidden;
            IsPaused = !IsPaused;

            CheckResult();
            DisplayRecords();

            Start();
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
                _pauseTextTextBlock.Visibility = Visibility.Visible;
                _restartTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                DispatcherTimer.Start();
                _pauseTextBlock.Text = "\uf04c";
                _pauseTextTextBlock.Visibility = Visibility.Hidden;
                _restartTextBlock.Visibility = Visibility.Hidden;
            }

            IsPaused = !IsPaused;
        }
        private void Exit(object sender, MouseButtonEventArgs e)
        {
            Close();

            FileManager.WriteToRecords(Records);
        }

        private void Update()
        {
            if (Settings.Difficulty == Difficulties.Python)
            {
                _area.Opacity = Snake.Starve / 0.6 / 100;
            }

            _area.Children.Clear();

            if (CanShowWalls)
            {
                foreach (var obstacle in Obstacles)
                {
                    _area.Children.Add(obstacle.AsRectangle);
                }
            }

            foreach (var square in Snake.GetValue())
            {
                _area.Children.Add(square);
            }

            foreach (var food in Food)
            {
                _area.Children.Add(food.AsRectangle);
            }


            Timer = Timer.AddMilliseconds(Settings.Speed);

            _timeTextBlock.Text = TimerToString;
        }

        private void CreateWalls()
        {
            ObstacleModel obstacle1 = new ObstacleModel(100, 100, new Point(200, 100));
            ObstacleModel obstacle2 = new ObstacleModel(100, 100, new Point(200, 300));

            Obstacles.Add(obstacle1);
            Obstacles.Add(obstacle2);
        }

        private void CheckResult()
        {
            if (Settings.IsDevModeOn) return;

            Records.Add(new RecordModel(Score.ToString(), TimerToString) { Difficulty = Settings.DifficultyToString });
        }

        private void DisplayRecords()
        {
            Records = RecordModel.OrderRecords(Records);

            _recordsListView.ItemsSource = Records.Take(10);
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (IsGameStarted && !IsPaused)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if (Snake.CurrentDirection != SnakeDirections.Down && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Up;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.Down:
                        if (Snake.CurrentDirection != SnakeDirections.Up && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Down;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.Left:
                        if (Snake.CurrentDirection != SnakeDirections.Right && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Left;
                        break;
                    case Key.Right:
                        if (Snake.CurrentDirection != SnakeDirections.Left && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Right;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.W:
                        if (Snake.CurrentDirection != SnakeDirections.Down && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Up;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.S:
                        if (Snake.CurrentDirection != SnakeDirections.Up && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Down;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.A:
                        if (Snake.CurrentDirection != SnakeDirections.Right && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Left;
                        DidAlreadySnakeMoved = true;
                        break;
                    case Key.D:
                        if (Snake.CurrentDirection != SnakeDirections.Left && !DidAlreadySnakeMoved)
                            Snake.CurrentDirection = SnakeDirections.Right;
                        DidAlreadySnakeMoved = true;
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
        private void OpenSettingsWindow(object sender, MouseButtonEventArgs e)
        {
            if (IsPaused || !IsGameStarted) new SettingsWindow().Show();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileManager.WriteToRecords(Records);
        }
    }
}
