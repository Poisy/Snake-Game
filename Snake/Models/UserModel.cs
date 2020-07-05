using System;
using System.Collections.Generic;

namespace Snake.Models
{
    public class UserModel
    {
        private readonly int[] LEVELXPREQUIMENT = {0, 10, 50, 100, 500, 1000, 2500, 5000, 10000, 15000, 25000, 50000, 100000, 250000, 500000, 750000, 1000000 };
        public byte Level
        {
            get
            {
                for (byte i = (byte)(LEVELXPREQUIMENT.Length-1); i > 0; i--)
                {
                    if (XP > LEVELXPREQUIMENT[i])
                    {
                        return i;
                    }
                }

                return 0;
            }
        }
        public byte LevelPercentage { get { return (byte)((XP / (double)LEVELXPREQUIMENT[Level + 1]) * 100); } }
        public long XP { get; set; }
        public long XPToNextLevel { get { return LEVELXPREQUIMENT[Level + 1] - XP; } }

        public UserModel()
        {
            LoadUserData();
        }

        public void SaveUserData()
        {
            Dictionary<string, string> userData = new Dictionary<string, string>();

            userData["xp"] = XP.ToString();
            userData["quest"] = QuestManager.MissionID.ToString();

            FileManager.WriteToUser(userData);
        }
        private void LoadUserData()
        {
            Dictionary<string, string> userData = FileManager.ReadFromUser();

            XP = Convert.ToInt64(userData["xp"]);
            QuestManager.MissionID = Convert.ToByte(userData["quest"]);
        }
    }
}
