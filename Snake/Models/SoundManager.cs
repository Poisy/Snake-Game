using Snake.Data;
using System;
using System.Windows.Media;

namespace Snake.Models
{
    public static class SoundManager
    {
        public static double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                Sound.Volume = volume;
                BGM.Volume = volume;
            }
        }

        private static double volume = 0.1;

        private static MediaPlayer Sound { get; } = new MediaPlayer();
        private static MediaPlayer BGM { get; } = new MediaPlayer();

        public static void PlaySound(Soundtrack sound)
        {
            string path;

            switch (sound)
            {
                case Soundtrack.Pop:
                    path = @"\Data\sounds\eat_food.mp3";
                    break;
                case Soundtrack.LevelUp:
                    path = @"\Data\sounds\level_up.mp3";
                    break;
                case Soundtrack.GameOver:
                    path = @"\Data\sounds\death.mp3";
                    break;
                default:
                    path = "";
                    break;
            }

            Sound.Open(new Uri(Environment.CurrentDirectory + path, UriKind.Relative));
            Sound.Volume = Volume;
            Sound.Play();
        }
        public static void PlayBGM(BackgroundMusic sound)
        {
            string path;

            switch (sound)
            {
                case BackgroundMusic.Gameplay:
                    path = @"\Data\sounds\bgm.mp3";
                    break;
                case BackgroundMusic.Menu:
                    path = @"\Data\sounds\bgm_2.mp3";
                    break;
                default:
                    path = "";
                    break;
            }

            BGM.Open(new Uri(Environment.CurrentDirectory + path, UriKind.Relative));
            BGM.Volume = Volume;
            BGM.MediaEnded += LoopBGM;
            BGM.Play();
        }

        public static void ChangeBGMSpeed(double speed)
        {
            BGM.Stop();
            BGM.SpeedRatio = speed;
            BGM.Play();
        }

        private static void LoopBGM(object sender, EventArgs e) => BGM.Position = TimeSpan.Zero;
    }
}
