using System;
using System.Collections.Generic;
using System.Text;

namespace Cross_Sums
{
    /**************************************************************
     * RecursiveSolver
     * The RecursiveSolver updates the possible values of copyOfUnsolvedCellList in
     * a cellToTry queue by attempting a solution and either confirming
     * the solution or throwing it out.
     *************************************************************/
    class RecursiveSolver  // Need to make this work out actual solutions to copyOfUnsolvedCellList...
    {
        public RecursiveSolver(List<int> solutionList,
                               int numberOfDigits,
                               int sum)
        {
            if (solutionList.Count == 0)
                return;
            bool solutionCouldWork = false;
            while (!solutionCouldWork)
            {
                foreach (int solution in solutionList)
                {
                    if (numberOfDigits <= 0)
                    {
                        solutionCouldWork = true;
                        break;
                    }
                    List<int> tempList = new List<int>(solutionList);
                    tempList.Remove(solution);
                    solutionCouldWork = Solve(tempList, numberOfDigits - 1, sum - solution);
                    if (!solutionCouldWork)
                    {
                        solutionList.Remove(solution);
                        if (solutionList.Count == 0)
                            return;
                        break;
                    }
                }
            }
        }

        public RecursiveSolver(List<OpenCell> list, int sum)
        {
               
            List<OpenCell> solutionList = list;
            int numberOfDigits = solutionList.Count;
            if (numberOfDigits == 0)
                return;

            bool solutionCouldWork = false;
            while (!solutionCouldWork)
            {
                foreach (OpenCell solution in solutionList)
                {
                    if (numberOfDigits <= 0)
                    {
                        solutionCouldWork = true;
                        break;
                    }
                    List<OpenCell> tempList = new List<OpenCell>(solutionList);
                    tempList.Remove(solution);
               //     solutionCouldWork = UpdateValueList(tempList, numberOfDigits - 1, sum - solution);
                    if (!solutionCouldWork)
                    {
                        solutionList.Remove(solution);
                        if (solutionList.Count == 0)
                            return;
                        break;
                    }
                }
            }
        }

        /****************************************************************
         * bool UpdateValueList()
         * Searches recursively through all copyOfUnsolvedCellList in a list, and tries
         * their possible solutions to see if there is a group that can
         * add to the given sumOfUnsolvedCells.  A trick of sorts is used where this
         * method returns true when asked about an empty list and a sumOfUnsolvedCells
         * of 0.  This only happens when a combination of values were
         * found that add to the original sumOfUnsolvedCells.
         ***************************************************************/

        private bool Solve(List<int> solutionList,  // how do I make solutionList read only?
                           int numberOfDigits,
                           int sum)
        {
            if ((sum == 0) && (numberOfDigits == 0))
                return true;
            if ((sum <= 0) || (numberOfDigits <= 0))
                return false;
            if (!DigitsCanAddToSum(numberOfDigits, sum))
                return false;

            List<int> localSolutionList = new List<int>(solutionList);
            numberOfDigits--;
            
            foreach (int solution in solutionList)
            {
                localSolutionList.Remove(solution);
                int sumToTry = sum - solution;
                bool solutionIsPossible = Solve(localSolutionList, numberOfDigits, sumToTry);
                if (solutionIsPossible)
                    return true;
            }
            return false;
        }

        private bool DigitsCanAddToSum(int numberOfDigits, int sum)
        {
            if (numberOfDigits < 1)
                return false;
            int highestPossibleSum = 0;
            int lowestPossibleSum = 0;
            for (int i = 1; i <= numberOfDigits; ++i)
            {
                lowestPossibleSum += i;
                highestPossibleSum += (10 - i);
            }
            if ( (lowestPossibleSum <= sum) && (sum <= highestPossibleSum) )
                return true;
            return false;
        }
    }
}
