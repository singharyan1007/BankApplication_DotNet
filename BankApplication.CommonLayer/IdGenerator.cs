using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
   
    public static class IdGenerator
    {
        private static readonly string filePath = "CurrentId.txt";

        public static string GenerateAccNo(string accType)
        {
            return $"{accType.Substring(0, 3).ToUpper()}{GenerateId()}";
        }
        public static int GenerateId()
        {
            int currentId = ReadCurrentId();
            int newId = currentId + 1;
            SaveCurrentId(newId);
            return newId;

        }

        private static int ReadCurrentId()
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "1000");
            }
            string idStr = File.ReadAllText(filePath);
            if (int.TryParse(idStr, out int currentId))
            {
                return currentId;
            }

            // If file content is corrupted, reset to 1000
            return 1000;
        }

        private static void SaveCurrentId(int newId)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(newId.ToString());
            }
        }
    }
}
