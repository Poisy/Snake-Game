using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Snake.Data;
using Snake.Models;
using Snake.Windows;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public UserModel User { get; } = new UserModel();
        private QuestManager QuestManager { get; set; } = new QuestManager();
        public SnakeModel Snake { get; set; }
        public List<FoodModel> Food { get; set; } = new List<FoodModel>();
        public List<ObstacleModel> Obstacles { get; set; } = new List<ObstacleModel>();
        public DispatcherTimer DispatcherTimer { get; set; } = new DispatcherTimer();
        public static bool CanShowWalls { get; set; } = false;
        public static bool IsGameStarted { get; set; } = false;
        public static bool IsPaused { get; set; } = false;
        private List<RecordModel> Records { get; set; } = FileManager.ReadFromRecords();
        private bool DidAlreadySnakeMoved { get; set; }
        private int Score
        {
            get { return score; }
            set { score += (100 + 50 * (int)Settings.Difficulty)*value; }
        }
        private byte ScoreMultiplier { get; set; } = 1;
        private DateTime Timer { get; set; }
        private string TimerToString => (Timer.Minute > 9 ? Timer.Minute.ToString() : "0" + Timer.Minute.ToString()) +
                ":" + (Timer.Second > 9 ? Timer.Second.ToString() : "0" + Timer.Second.ToString());
        private short DoubleScoreEffectTimer { get; set; } = -1;
        private short FastMoveEffectTimer { get; set; } = -1;
        private short SlowMoveEffectTimer { get; set; } = -1;
        private int score = 0;

        public MainWindow()
        {
            InitializeComponent();

            SoundManager.PlayBGM(BackgroundMusic.Menu);

            DisplayRecords();
            UpdateQuest();

            DispatcherTimer.Tick += MainLoop;
        }

        private void MainLoop(object sender, EventArgs e)
        {
            CheckEffects();

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
                    SoundManager.PlaySound(Soundtrack.Pop);

                    Snake.ExtendBody();

                    switch (food.Type)
                    {
                        case FoodType.Normal:
                            food.NewPosition(Snake);
                            Score = ScoreMultiplier;
                            QuestManager.TrySaveToXp(User, ScoreMultiplier);
                            break;
                        case FoodType.Gold:
                            Food.Remove(food);
                            Score = ScoreMultiplier;
                            QuestManager.TrySaveToXp(User, ScoreMultiplier);
                            FoodModel.ChangeFoodColor(Food, Brushes.Gold);
                            DoubleScoreEffect(10);
                            break;
                        case FoodType.Special:
                            Food.Remove(food);
                            Score = ScoreMultiplier * 2;
                            QuestManager.TrySaveToXp(User, ScoreMultiplier * 2);
                            SpecialEffect(10);
                            break;
                    }

                    _scoreTextBlock.Text = Score.ToString();

                    for (int i = 0; i < Settings.FoodSpawnRate - 1; i++)
                    {
                        FoodModel newFood = new FoodModel();
                        newFood.NewPosition(Snake);

                        Food.Add(newFood);
                    }

                    FoodModel.ChanceOtherFoodToAppear(Food, Snake);

                    break;
                }
            }

            Update();
        }

        public void RefreshGame()
        {
            SetDeffaultSettings();

            IsGameStarted = false;
            IsPaused = false;

            _area.Children.Clear();
            DispatcherTimer.Stop();

            Snake = new SnakeModel();
            Food = new List<FoodModel>();

            HideAreaTextBlocks();

            _startTextBlock.Visibility = Visibility.Visible;
            _titleTextBlock.Visibility = Visibility.Visible;

            _pauseTextTextBlock.Visibility = Visibility.Hidden;
        }

        private void Start(object sender, MouseButtonEventArgs e)
        {
            if (IsGameStarted) Restart();
            else Start();
        }
        private void Start()
        {
            SoundManager.PlayBGM(BackgroundMusic.Gameplay);
            SoundManager.ChangeBGMSpeed(1);

            IsGameStarted = true;

            SetDeffaultSettings();

            QuestManager.SaveQuest();

            QuestManager.TryStartNewGame();

            Update();

            DispatcherTimer.Start();
        }
        private void Stop()
        {
            IsGameStarted = false;

            SoundManager.PlaySound(Soundtrack.GameOver);
            SoundManager.PlayBGM(BackgroundMusic.Menu);
            SoundManager.ChangeBGMSpeed(1);

            _reasonDiedTextBlock.Text = Snake.ReasonDied;
            _scoreResultTextBlock.Text = $"Time: {TimerToString}       Score: {Score}";

            _area.Children.Clear();
            DispatcherTimer.Stop();

            Snake = new SnakeModel();
            Food = new List<FoodModel>();

            _pauseTextBlock.Text = "\uf04b";

            ShowAreaTextBlocks();

            QuestManager.TryStartNewGame();
            UpdateQuest();

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

            QuestManager.SaveQuest();

            QuestManager.CheckQuest(User, Score, Timer);
            QuestManager.Quest.ClearCurrentValue();
            UpdateQuest();

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
            FileManager.WriteToRecords(Records);
            User.SaveUserData();

            Application.Current.Shutdown();
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

            QuestManager.CheckQuest(User, Score, Timer);

            UpdateQuest();

            Timer = Timer.AddMilliseconds(DispatcherTimer.Interval.Milliseconds);

            _timeTextBlock.Text = TimerToString;
        }
        private void UpdateQuest()
        {
            _levelTextBlock.Text = "Level " + User.Level.ToString();
            _xpProgressBar.Value = User.LevelPercentage;
            _xpProgressBar.ToolTip = new TextBlock() { Text = User.XPToNextLevel.ToString() + " xp to next level" };
            _xpTextBlock.Text = User.XP.ToString() + " xp";

            if (QuestManager.IsFreeModeOn) return;
            _questTextBlock.Text = "Quest " + QuestManager.MissionID.ToString();
            _questTaskTextBlock.Text = $"{QuestManager.Quest.Type} {QuestManager.Quest.Goal} " + (QuestManager.Quest.Type == "Survive" ? "sec" : "pts");
            _questTaskRemainTextBlock.Text = QuestManager.Quest.Remain.ToString() + (QuestManager.Quest.Type == "Survive" ? " sec" : " pts");
            _questDifficultyTextBlock.Text = QuestManager.Quest.Difficulty;
            _questRewardTextBlock.Text = QuestManager.Quest.Reward.ToString() + " xp";
        }

        private void SetDeffaultSettings()
        {
            if (FastMoveEffectTimer >= 0) Settings.Speed /= 2;
            if (SlowMoveEffectTimer >= 0) Settings.Speed *= 2;

            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);

            DoubleScoreEffectTimer = -1;
            FastMoveEffectTimer = -1;
            SlowMoveEffectTimer = -1;

            ScoreMultiplier = 1;

            FoodModel.ChangeFoodColor(Food, Brushes.Red);

            FoodModel.IsGoldenFoodShown = false;
            FoodModel.IsSpecialFoodShown = false;

            _scoreTextBlock.Text = "0";
            _timeTextBlock.Text = "00:00";
            _area.Background = Settings.Background;
            _area.Opacity = 1;

            HideAreaTextBlocks();

            score = 0;
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

            Records.Add(new RecordModel(Score.ToString(), TimerToString) { Difficulty = Settings.DifficultyToChar });
        }

        private void DisplayRecords()
        {
            Records = RecordModel.OrderRecords(Records);

            _recordsListView.ItemsSource = Records.Take(10);
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

        private void CheckEffects()
        {
            if (DoubleScoreEffectTimer >= 0)
            {
                if (DoubleScoreEffectTimer == 0)
                {
                    ScoreMultiplier = 1;
                    FoodModel.IsGoldenFoodShown = false;
                    FoodModel.ChangeFoodColor(Food, Brushes.Red);
                }
                DoubleScoreEffectTimer--;
            }
            else if (FastMoveEffectTimer >= 0)
            {
                if (FastMoveEffectTimer == 0)
                {
                    Settings.Speed /= 2;
                    DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);
                    FoodModel.IsSpecialFoodShown = false;

                    SoundManager.ChangeBGMSpeed(1);

                }
                FastMoveEffectTimer--;
            }
            else if (SlowMoveEffectTimer >= 0)
            {
                if (SlowMoveEffectTimer == 0)
                {
                    Settings.Speed *= 2;
                    DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);
                    FoodModel.IsSpecialFoodShown = false;

                    SoundManager.ChangeBGMSpeed(1);

                }
                SlowMoveEffectTimer--;
            }
        }
        private void DoubleScoreEffect(byte seconds)
        {
            ScoreMultiplier = 2;
            DoubleScoreEffectTimer = (short)(1000*seconds / Settings.Speed);
        }
        private void SpecialEffect(byte seconds)
        {
            if (new Random().Next(0, 2) == 1)
            {
                Settings.Speed *= 2;
                FastMoveEffectTimer = (short)(1000 * seconds / Settings.Speed);

                SoundManager.ChangeBGMSpeed(0.5);

            }
            else
            {
                Settings.Speed /= 2;
                SlowMoveEffectTimer = (short)(1000 * seconds / Settings.Speed);

                SoundManager.ChangeBGMSpeed(2);
            }

            DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Speed);
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

        private void OpenDeveloperWindow(object sender, MouseButtonEventArgs e)
        {
            if (IsPaused || !IsGameStarted) new DeveloperWindow().Show();
        }
        private void OpenSettingsWindow(object sender, MouseButtonEventArgs e)
        {
            if (IsPaused || !IsGameStarted) new SettingsWindow(this).Show();
        }
        private void OpenInformationWindow(object sender, MouseButtonEventArgs e)
        {
            if (IsPaused || !IsGameStarted) new InformationWindow().Show();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileManager.WriteToRecords(Records);
            User.SaveUserData();
        }
    }
}
