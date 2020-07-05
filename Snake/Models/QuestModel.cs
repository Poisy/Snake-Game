namespace Snake.Models
{
    public class QuestModel
    {
        public string Type { get; private set; }
        public int Goal { get; private set; }
        public string Difficulty { get; private set; }
        public int Reward { get; private set; }
        public int Current { get; private set; } = 0;
        public int Remain => Goal - Current < 0 ? 0 : Goal - Current;
        public bool IsOneTime => Type != "Collect";
        public bool IsCompleted { get; private set; } = false;

        private int SavedValue { get; set; } = 0;

        public QuestModel(string type, int goal, string difficulty, int reward)
        {
            Type = type;
            Goal = goal;
            Difficulty = difficulty;
            Reward = reward;
        }

        public void Update(int value)
        {
            Current = value + SavedValue;

            if (Current >= Goal) IsCompleted = true;
        }

        public void ClearCurrentValue()
        {
            if (IsOneTime && !IsCompleted) Current = 0;
        }

        public void Save()
        {
            if (!IsOneTime) SavedValue = Current;
        }
    }
}
