using Snake.Data;
using Snake.Windows;
using System;
using System.Windows;

namespace Snake.Models
{
    public class QuestManager
    {
        public QuestModel Quest { get; private set; }
        public static byte MissionID { get; set; }
        public bool IsFreeModeOn { get; private set; } = false;
        public bool IsThereQuest => !Quest.IsCompleted;

        public QuestManager()
        {
            StartQuest();
        }

        public void CheckQuest(UserModel user, int score, DateTime timer)
        {
            if (!Settings.IsDevModeOn)
            {
                if (IsFreeModeOn) return;

                if ((Settings.DifficultyToString == Quest.Difficulty || Quest.Difficulty == "-") && IsThereQuest)
                {
                    switch (Quest.Type)
                    {
                        case "Score":
                            Quest.Update(score);
                            break;
                        case "Survive":
                            Quest.Update(timer.Second + timer.Minute * 60);
                            break;
                        case "Collect":
                            Quest.Update(score);
                            break;
                    }

                    if (Quest.IsCompleted)
                    {
                        user.XP += Quest.Reward;

                        if (++MissionID >= 17)
                        {
                            MissionID = 0;
                            string text = "Congrats\n You complete every mission\nNow you will gain xp equal to the points you get from the food\n";

                            new DialogWindow(text).ShowDialog();
                        }
                    }
                }
            }
        }

        private void StartQuest()
        {
            switch (MissionID)
            {
                case 0:
                    IsFreeModeOn = true;
                    break;
                case 1:
                    Quest = new QuestModel("Score", 500, "-", 10);
                    break;
                case 2:
                    Quest = new QuestModel("Score", 1000, "Easy", 25);
                    break;
                case 3:
                    Quest = new QuestModel("Survive", 60, "-", 50);
                    break;
                case 4:
                    Quest = new QuestModel("Collect", 2500, "-", 100);
                    break;
                case 5:
                    Quest = new QuestModel("Score", 2000, "Normal", 250);
                    break;
                case 6:
                    Quest = new QuestModel("Collect", 10000, "-", 500);
                    break;
                case 7:
                    Quest = new QuestModel("Survive", 60, "Hard", 500);
                    break;
                case 8:
                    Quest = new QuestModel("Score", 2500, "Hard", 750);
                    break;
                case 9:
                    Quest = new QuestModel("Collect", 20000, "-", 1000);
                    break;
                case 10:
                    Quest = new QuestModel("Survive", 60, "Python", 1000);
                    break;
                case 11:
                    Quest = new QuestModel("Score", 5000, "Normal", 1000);
                    break;
                case 12:
                    Quest = new QuestModel("Collect", 25000, "-", 1500);
                    break;
                case 13:
                    Quest = new QuestModel("Survive", 120, "Python", 2000);
                    break;
                case 14:
                    Quest = new QuestModel("Score", 5000, "Hard", 2500);
                    break;
                case 15:
                    Quest = new QuestModel("Collect", 30000, "-", 3000);
                    break;
                case 16:
                    Quest = new QuestModel("Score", 5000, "Python", 5000);
                    break;
            }
        }

        public void TryStartNewGame()
        {
            if (IsFreeModeOn) return;
            if (Quest.IsCompleted) StartQuest();
        }

        public void SaveQuest()
        {
            if (IsFreeModeOn) return;
            Quest.Save();
        }

        public void TrySaveToXp(UserModel user, int score)
        {
            if (IsFreeModeOn)
            {
                user.XP += (100 + 50 * (int)Settings.Difficulty) * score;
            }
        }
    }
}
