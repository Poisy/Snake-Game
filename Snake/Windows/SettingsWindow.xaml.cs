using Snake.Data;
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
        public SettingsWindow()
        {
            InitializeComponent();

            _difficultyComboBox.SelectedIndex = (int)Settings.Difficulty;
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
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
