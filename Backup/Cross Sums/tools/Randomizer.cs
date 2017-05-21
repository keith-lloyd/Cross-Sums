using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    class Randomizer
    {
        public Randomizer(int min, int max)
        {
            valueList = new List<int>();
            for (int i = min; i <= max; ++i)
            {
               valueList.Add(i);
            }
       }

        public int Next()
        {
            if (valueList.Count() == 0)
                return 0;
            int index = random.Next(valueList.Count());
            int selection = valueList.ElementAt(index);
            valueList.RemoveAt(index);
            return selection;
        }

        public static int RandomSelectFrom(List<int> selectionList)
        {
            if (selectionList.Count() == 0)
                return 0;
            int index = random.Next(selectionList.Count());
            int selection = selectionList.ElementAt(index);
            return selection;
        }

        private static Random random = new Random();
        List<int> valueList;
    }
}
