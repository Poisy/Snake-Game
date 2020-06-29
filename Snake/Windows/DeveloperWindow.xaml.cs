using Snake.Data;
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
using System.Windows.Shapes;


namespace Snake.Windows
{
    /// <summary>
    /// Interaction logic for DeveloperWindow.xaml
    /// </summary>
    public partial class DeveloperWindow : Window
    {
        public DeveloperWindow()
        {
            int speed = Settings.Speed;
            int foodspawnrate = Settings.FoodSpawnRate;
            int foodspawncount = Settings.FoodSpawnCount;

            InitializeComponent();

            _devCheckBox.IsChecked = Settings.IsDevModeOn;

            _snakeSpeedSlider.Value = speed;
            _foodSpawnRateSlider.Value = foodspawnrate;
            _foodSpawnCountSlider.Value = foodspawncount;
            _snakeHeadColorPicker.SelectedColor = ((SolidColorBrush)Settings.HeadColor).Color;
            _snakeBodyColorPicker.SelectedColor = ((SolidColorBrush)Settings.BodyColor).Color;
            _backgroundColorPicker.SelectedColor = ((SolidColorBrush)Settings.Background).Color;
            _foodColorPicker.SelectedColor = ((SolidColorBrush)Settings.FoodColor).Color;

            DevOptionsControl(Settings.IsDevModeOn);
        }

        private void SpeedChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Speed = (short)e.NewValue;
            _snakeSpeedSlider.Value = e.NewValue;
        }
        private void SnakeHeadColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.HeadColor = new SolidColorBrush(e.NewValue.Value);
            _snakeHeadColorPicker.SelectedColor = e.NewValue;
        }
        private void SnakeBodyColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.BodyColor = new SolidColorBrush(e.NewValue.Value);
            _snakeBodyColorPicker.SelectedColor = e.NewValue;
        }

        private void BackgroundColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.Background = new SolidColorBrush(e.NewValue.Value);
            _backgroundColorPicker.SelectedColor = e.NewValue;
        }

        private void FoodSpawnRateChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.FoodSpawnRate = (byte)e.NewValue;
            _foodSpawnRateSlider.Value = e.NewValue;
        }
        private void FoodSpawnCountChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.FoodSpawnCount = (byte)e.NewValue;
            _foodSpawnCountSlider.Value = e.NewValue;
        }
        private void FoodColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Settings.FoodColor = new SolidColorBrush(e.NewValue.Value);
            _foodColorPicker.SelectedColor = e.NewValue;
        }

        private void DevCheckBox(object sender, RoutedEventArgs e)
        {
            DevOptionsControl(_devCheckBox.IsChecked.Value);
        }
        private void DevOptionsControl(bool needToShow)
        {
            Settings.IsDevModeOn = needToShow;

            _snakeOptions.IsEnabled = needToShow;
            _gameOptions.IsEnabled = needToShow;
            _foodOptions.IsEnabled = needToShow;
        }
    }
}
