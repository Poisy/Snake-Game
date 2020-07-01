using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Data
{
    static class DyingReasons
    {
        public static string HitTheWall
        {
            get
            {
                string[] responds = { "Watch where you going, you hit a wall", 
                    "You just hit a wall", 
                    "The wall killed you", 
                    "The wall is stronger than you" };
                return responds[new Random().Next(0, responds.Length - 1)];
            }
        }
        public static string BiteYourSelf
        {
            get
            {
                string[] responds = { "Don't bite yourself!",
                    "How do you taste ?",
                    "You just bite yourself",
                    "Eat the food, not yourself"};
                return responds[new Random().Next(0, responds.Length - 1)];
            }
        }

        public static string Starving
        {
            get
            {
                string[] responds = { "Starved to death",
                    "Died from starving",
                    "Not enough food",
                    "Eat a snickers"};
                return responds[new Random().Next(0, responds.Length - 1)];
            }
        }
    }
}
