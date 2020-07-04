using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Snake.Models
{
    class RecordModel
    {
        public string Score { get; set; }
        public int ScoreToInt { get { return Convert.ToInt32(Score); } }
        public string Time { get; set; }
        public DateTime Date { get; set; }
        public byte Place { get; set; }
        public Brush Foreground { get; set; }
        public char Difficulty { get; set; }

        public RecordModel(string score, string time) : this(score, time, DateTime.Now) { }
        public RecordModel(string score, string time, DateTime date)
        {
            Score = score;
            Time = time;
            Date = date;
        }

        public static List<RecordModel> OrderRecords(List<RecordModel> records)
        {
            records = records.OrderBy(record => -record.ScoreToInt).ToList();

            for (byte i = 1; i <= records.Count; i++)
            {
                records[i-1].Place = i;
                records[i - 1].Foreground = Brushes.Black;
            }

            if (records.Count >= 3)
            {
                records[0].Foreground = new SolidColorBrush(Color.FromRgb(235, 64, 52));
                records[1].Foreground = new SolidColorBrush(Color.FromRgb(178, 64, 52));
                records[2].Foreground = new SolidColorBrush(Color.FromRgb(100, 64, 52));
            }

            return records;
        }

        public static List<RecordModel> DictionaryToRecords(Dictionary<string, string[]> dict)
        {
            List<RecordModel> records = new List<RecordModel>();

            foreach (var record in dict)
            {
                records.Add(new RecordModel(record.Value[0], record.Value[1], DateTime.Parse(record.Key)) { Difficulty = record.Value[2][0] });
            }

            return OrderRecords(records);
        }

        public static Dictionary<string, string[]> ToDictionary(List<RecordModel> records)
        {
            Dictionary<string, string[]> dictRecords = new Dictionary<string, string[]>();

            foreach (var record in records)
            {
                dictRecords[record.Date.ToString()] = new string[] { record.Score, record.Time, record.Difficulty.ToString() };
            }

            return dictRecords;
        }
    }
}
