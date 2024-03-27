using System;
using System.Collections.Generic;
using System.Linq;

namespace AI
{
    public static class AINameGenerator
    {
        private static readonly List<string> MixedNames = new()
        {
            "Ahmed", "Mohammed", "Ali", "Hassan", "Omar",
            "Ibrahim", "Fatima", "Noor", "Aisha", "Sara",
            "Bandar", "Yusuf", "Hussein", "Nadia", "Layla",
            "Majid", "Rashid", "Zainab", "Faisal", "Amira",
            "Christopher", "John", "Mary", "Paul", "Lucy",
            "George", "Catherine", "Elizabeth", "Michael", "Anna"
        };

        public static List<string> GetNameList()
        {
            var rnd = new Random();
            return MixedNames.OrderBy(x => rnd.Next()).ToList();
        }

        public static string GetName()
        {
            var nameList = GetNameList();
            // Get random number
            var rnd = new Random();
            var index = rnd.Next(0, nameList.Count);
            // Get name at random index
            var name = nameList[index];
            return name;
        }
    }
}