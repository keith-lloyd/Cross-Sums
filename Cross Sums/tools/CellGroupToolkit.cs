using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    class CellGroupToolkit
    {
        /*************************************************************************
         *                             LeastPossibleSum
         *************************************************************************
         *   This method makes a list of digits found in all copyOfUnsolvedCellList of the passed
         *   in cellToTry list to their number of occurances.  Then, it considers the
         *   n smallest digits, where n is the number of copyOfUnsolvedCellList to be filled in.
         *************************************************************************/
        public static int LeastPossibleSum(List<OpenCell> cellListToCopy)
        {
            int sum = 0;
            List<int> lowestDigits = FindTheLowestDigits(cellListToCopy);
            foreach (int digit in lowestDigits)
                sum += digit;
            CrossSumsForm.debugLog.Write("          (Least) Sum: " + sum + "\n");
            return sum;
        }

        /*************************************************************************
         *                          GreatestPossibleSum
         *************************************************************************
         *   See description for LeastPossibleSum, above.
         *************************************************************************/
        public static int GreatestPossibleSum(List<OpenCell> cellListToCopy)
        {
            int sum = 0;
            List<int> highestDigits = FindTheHighestDigits(cellListToCopy);
            foreach (int digit in highestDigits)
                sum += digit;
            CrossSumsForm.debugLog.Write("         (Greatest) Sum: " + sum + "\n");
            return sum;
        }

        //public static bool RecursivelyRemoveInconsistentDigits(List<OpenCell> cellList, int sumOfCells)
        //{
        //    if ( (cellList.Count == 1) && (cellList.ElementAt(0).ValueIsInPossibleValueList(sumOfCells)) )
        //        return true;
        //    else
        //        return false;
        //}

        public static List<int> FindTheLowestDigits(List<OpenCell> cellListToCopy)
        {
            List<OpenCell> cellList = cellListToCopy.ToList();
            List<int> lowestDigits = new List<int>();
            if (cellListToCopy.Count > 0)
                RecursiveDigitFinder(false, cellList, lowestDigits);
            return lowestDigits;
        }

        public static List<int> FindTheHighestDigits(List<OpenCell> cellListToCopy)
        {
            List<OpenCell> cellList = new List<OpenCell>(cellListToCopy);
            List<int> highestDigits = new List<int>();
            if (cellListToCopy.Count > 0)
                RecursiveDigitFinder(true, cellList, highestDigits);
            return highestDigits;
        }

        private static void RecursiveDigitFinder(bool keepLargestDigits,
                                                 List<OpenCell> cellList,
                                                 List<int> foundDigits)
        {
            int numberOfDigitsToSum = cellList.Count();
            int[] digitToNumberOfOccurances = CountDigits(cellList);
            foreach(int foundDigit in foundDigits)
                digitToNumberOfOccurances[foundDigit] = 0;
            int numberOfDistinctDigits = TotalNumberOfDistinctDigits(digitToNumberOfOccurances);
            if (numberOfDistinctDigits < numberOfDigitsToSum)
            {
                CrossSumsForm.debugLog.Write("RecursiveDigitFinder found not enough digits for cellToTry group.\n");
                CrossSumsForm.debugLog.Write("   cellToTry values: " + PrintAllCellValues(cellList) + "\n");
                return;
            }

            int numberOfDigitsToRemove = numberOfDistinctDigits - numberOfDigitsToSum;

            if (keepLargestDigits)
                PurgeLowestDigits(numberOfDigitsToRemove, digitToNumberOfOccurances);
            else
                PurgeHighestDigits(numberOfDigitsToRemove, digitToNumberOfOccurances);

            int digitToRemove = SelectMostInfrequentDigit(digitToNumberOfOccurances, keepLargestDigits);

            foundDigits.Add(digitToRemove);
            RemoveCellContainingAGivenDigit(cellList, digitToRemove);
            if (cellList.Count > 0)
                RecursiveDigitFinder(keepLargestDigits, cellList, foundDigits);
            return;
        }

        private static int TotalNumberOfDistinctDigits(int[] occurancesPerDigit)
        {
            int digitCount = 0;
            for (int digit = 1; digit <= 9; ++digit)
            {
                if (occurancesPerDigit[digit] != 0)
                    ++digitCount;
            }
            return digitCount;
        }

        private static void PrintDigitArray(int[] digitArray)
        {
            for (int digit = 1; digit <= 9; ++digit)
            {
                if (digitArray[digit] != 0)
                    CrossSumsForm.debugLog.Write("      " + digit + " appears " + digitArray[digit] + " times.\n");
            }
        }

        private static void PurgeHighestDigits(int numberOfDigitsToRemove, int[] digitToNumberOfOccurances)
        {
            for (int digit = 9; (digit >= 1) && (numberOfDigitsToRemove > 0); --digit)
            {
                if (digitToNumberOfOccurances[digit] != 0)
                {
                    digitToNumberOfOccurances[digit] = 0;
                    --numberOfDigitsToRemove;
                }
            }
        }

        private static void PurgeLowestDigits(int numberOfDigitsToRemove, int[] digitToNumberOfOccurances)
        {
            for (int digit = 1; (digit <= 9) && (numberOfDigitsToRemove > 0); ++digit)
            {
                if (digitToNumberOfOccurances[digit] != 0)
                {
                    digitToNumberOfOccurances[digit] = 0;
                    --numberOfDigitsToRemove;
                }
            }
        }

        private static int SelectMostInfrequentDigit(int[]  digitToNumberOfOccurances,
                                                     bool   preferLargerDigits)
        {
            int chosenDigit = 0;
            for (int digit = 1; digit <= 9; ++digit)
            {
                int occurences = digitToNumberOfOccurances[digit];
                if (occurences == 0)
                    continue;
                if (chosenDigit == 0)
                {
                    chosenDigit = digit;
                    continue;
                }
                if (occurences < digitToNumberOfOccurances[chosenDigit])
                    chosenDigit = digit;
                if (preferLargerDigits && (occurences == digitToNumberOfOccurances[chosenDigit]))
                    chosenDigit = digit;
            }
            return chosenDigit;
        }

        private static void RemoveCellContainingAGivenDigit (List<OpenCell> cellList, int digitToRemove)
        {
            foreach (OpenCell cell in cellList)
            {
                if (cell.ValueIsInPossibleValueList(digitToRemove))
                {
                    cellList.Remove(cell);
                    break;
                }
            }
        }

        private static int[] CountDigits(List<OpenCell> cellList)
        {
            int[] digitToNumberOfOccurances = new int[10];
            foreach (OpenCell cell in cellList)
            {
                foreach (int digit in cell.GetPossibleValues())
                    ++digitToNumberOfOccurances[digit];
            }
            return digitToNumberOfOccurances;
        }

        private static string PrintAllCellValues(List<OpenCell> cellListToPrint)
        {
            string returnStr = "Cell Values: [";
            foreach (OpenCell cell in cellListToPrint)
            {
                returnStr += cell.ToString() + " ";
            }
            return returnStr + "]";
        }

    } // End class
}
