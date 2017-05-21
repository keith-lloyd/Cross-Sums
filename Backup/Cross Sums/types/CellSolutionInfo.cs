using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums.types
{
    public class CellSolutionInfo
    {
        public CellSolutionInfo()
        {
            possibleValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            backupValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public void DigitIsNotAPossibleValue(int digit)
        {
            possibleValues.Remove(digit);
        }

        public bool Contains(int digit)
        {
            return possibleValues.Contains(digit);
        }

        public List<int> GetPossibleValues()
        {
            return possibleValues;
        }

        public void ForceToAValue(int value)
        {
            possibleValues = new List<int> { value };
        }

        public int NumberOfPossibleDigits()
        {
            return possibleValues.Count();
        }

        public void BackUp()
        {
            backupValues = possibleValues.ToList();
        }

        public void Restore()
        {
            possibleValues = backupValues.ToList();
        }

        public void IntersectWith(List<int> integerList)
        {
            IEnumerable<int> intersection = possibleValues.Intersect(integerList);
            possibleValues = intersection.ToList();  // If this list shrinks, we have moved closer to a solution.  The returnInfo does not reflect this.

        }

        private List<int> possibleValues;
        private List<int> backupValues;
    }
}
