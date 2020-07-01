using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Snake.Data
{
    static class Settings
    {
        public static bool IsDevModeOn { get; set; } = false;

        public static Difficulties Difficulty { get; set; } = Difficulties.Normal;

        public static short Speed
        {
            get
            {
                if (IsDevModeOn) return speed;
                return 50;
            }
            set { speed = value; }
        }
        public static Brush HeadColor 
        {
            get
            {
                if (IsDevModeOn) return headColor;
                return Brushes.White;
            }
            set { headColor = value; }
        }
        public static Brush BodyColor
        {
            get
            {
                if (IsDevModeOn) return bodyColor;
                return Brushes.White;
            }
            set { bodyColor = value; }
        }

        public static Brush Background 
        {
            get
            {
                if (IsDevModeOn) return background;
                return Brushes.Black;
            }
            set { background = value; }
        }

        public static byte FoodSpawnRate 
        {
            get
            {
                if (IsDevModeOn) return foodSpawnRate;
                return 1;
            }
            set { foodSpawnRate = value; }
        }
        public static byte FoodSpawnCount 
        {
            get
            {
                if (IsDevModeOn) return foodSpawnCount;
                return 1;
            }
            set { foodSpawnCount = value; }
        }
        public static Brush FoodColor 
        {
            get
            {
                if (IsDevModeOn) return foodColor;
                return Brushes.Red;
            }
            set { foodColor = value; }
        }

        private static short speed  = 50;
        private static Brush headColor = Brushes.White;
        private static Brush bodyColor = Brushes.White;
        private static Brush background = Brushes.Black;
        private static byte foodSpawnRate = 1;
        private static byte foodSpawnCount = 1;
        private static Brush foodColor = Brushes.Red;
    }
}
