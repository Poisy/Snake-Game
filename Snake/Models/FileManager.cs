using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Snake.Models
{
    static class FileManager
    {
        private static string UserFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string SnakeFolder { get; } = UserFolder + @"\.poi\snake";

        public static List<RecordModel> ReadFromRecords()
        {
            Dictionary<string, string[]> dictToReturn;
            string file = SnakeFolder + @"\records.poi";

            TryCreateSnakeFolder();
            if (TryCreateRecordsFile(file))
            {
                return new List<RecordModel>();
            }
            
            using (StreamReader temp = new StreamReader(file))
            {
                dictToReturn = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(Security.Decrypt(temp.ReadToEnd(), Environment.MachineName.GetHashCode().ToString()));
            }

            return RecordModel.DictionaryToRecords(dictToReturn);
        }

        public static void WriteToRecords(List<RecordModel> records)
        {
            File.WriteAllText(SnakeFolder + @"\records.poi", Security.Encrypt(JsonConvert.SerializeObject(RecordModel.ToDictionary(records)), Environment.MachineName.GetHashCode().ToString()));
        }

        private static void TryCreateSnakeFolder()
        {
            if (!Directory.Exists(SnakeFolder))
            {
                Directory.CreateDirectory(SnakeFolder);
            }
        }
        private static bool TryCreateRecordsFile(string file)
        {
            if (!File.Exists(file))
            {
                using (StreamWriter temp = new StreamWriter(file, true)) { }

                return true;
            }

            return false;
        }
    }
}
