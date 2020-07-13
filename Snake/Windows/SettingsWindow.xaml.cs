using Snake.Data;
using Snake.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Snake.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        MainWindow MainWindow { get; set; }

        public SettingsWindow(MainWindow window)
        {
            MainWindow = window;

            InitializeComponent();

            _difficultyComboBox.SelectedIndex = (int)Settings.Difficulty;
            _volumeSlider.Value = SoundManager.Volume * 10;
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;

            switch (item.DataContext.ToString())
            {
                case "easy":
                    Settings.Difficulty = Difficulties.Easy;
                    break;
                case "normal":
                    Settings.Difficulty = Difficulties.Normal;
                    break;
                case "hard":
                    Settings.Difficulty = Difficulties.Hard;
                    break;
                case "python":
                    Settings.Difficulty = Difficulties.Python;
                    break;
            }

            MainWindow.RefreshGame();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundManager.Volume = e.NewValue / 10;
            SoundManager.ChangeBGMSpeed(1);
        }
    }
}
