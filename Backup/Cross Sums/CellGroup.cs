using System;
using System.Collections.Generic;
using System.Linq;
using Cross_Sums.types;

namespace Cross_Sums
{
    public abstract class CellGroup
    {
        public CellGroup(SumCell sumCell, List<OpenCell> cellList, DebugLog debugLog)
        {
            this.debugLog = debugLog;
            this.sumCell = sumCell;
            data = new GroupData(cellList);
            foreach (OpenCell cell in data.cellList)
                cell.AddParentGroup(this);
        }

        public static string PrintAllRowSums()
        {
            string returnStr = "   Row sums:";
            foreach (CellGroup group in rowList)
            {
                returnStr += " " + group.SumOfAllCells();
            }
            returnStr += "\n";
            return returnStr;
        }

        public static string PrintAllColumnSums()
        {
            string returnStr = "   Column sums:";
            foreach (CellGroup group in columnList)
            {
                returnStr += " " + group.SumOfAllCells();
            }
            returnStr += "\n";
            return returnStr;
        }

 
        public static List<CellGroup> rowList = new List<CellGroup>();
        public static List<CellGroup> columnList = new List<CellGroup>();

        public bool CannotBeSolved()
        {
            return ((SumIsKnown()) && (data.NumberOfUnsolvedCells() > 0)) ? true : false;
        }

        /******************************************************
        *                  SetTheSum
        ******************************************************
        * Chooses a random solution from the possibilities.  This method assumes
        * the entire puzzle is in a state where all invalid values have been
        * tossed out.
        * 
        * This method gets the cell group to a point where it is internally consistent.
        * It is up to the caller of this method to ensure that the rest of the puzzle
        * still works with the changes made here.
        * 
        * Pick any valid digit for the cellToTry.  See if the cellToTry can be forced to
        * that digit by:
        * 1) setting the sum of this group (assuming it hasn't already been set)
        * 2) setting the sum of the orthoganal group (again, if it hasn't been set.)
        * 3) Seeing if minimizing this group's sum and maximizing the other's works.
        * 4) Maximize this group and minimize the other, and see if that works.
        * 5) if nothing works, return false and try a different cellToTry.
        ******************************************************/
        public UpdateResult SetTheSum()
        {
            debugLog.Write("Method SetTheSum called...\n");
            BackUpData();
//            UpdateResult result = new UpdateResult();

            if (data.NumberOfUnsolvedCells() == 0)
            {
                if (!SumIsKnown())
                {
                    debugLog.Write("Returning for empty unsolvedCellList.  Had to set the sum.\n");
                    if (!UpdateSum(SumOfAllSolvedCells()))
                        throw new UnsolvableGroupException2
                            ("The sum of all the cells apparently doesn't work for this group.");
                    return UpdateResult.changed;
                }
                debugLog.Write("Returning for empty unsolvedCellList.\n");
                return UpdateResult.unchanged;
            }

            if (SumIsKnown())
            {
                debugLog.Write("Returning for already determined sum.\n");
                PrintAllCells();
                return UpdateResult.unchanged;
            }

            UpdateAvailableValueList();  // If this changes the group, is it bad to return unchanged later on?.
            PrintAllCells();

            if (OneUnsolvedCellLeft())
            {
                if (!SolveForLastCell())
                    throw new UnsolvableGroupException2("Tried the one possible solution, and bombed out.");        
                return UpdateResult.changed;
            }

            OpenCell cellToSolve = data.GetCopyOfUnsolvedCells().ElementAt(0);  // To Do:  Need to make this truly random.

            if (cellToSolve.TendencyOfCell() == Tendency.low)
            {
                int sum = 0;
                bool successful = MaximumSum(out sum);
                if (!successful)
                    throw new UnsolvableGroupException2("MaximumSum couldn't find anything.");
                if (!UpdateSum(sum))
                    throw new IllegalSumException2("Setting to maximum failed.");
            }
            else
            {
                int sum = 0;
                bool successful = MinimumSum(out sum);
                if (!successful)
                    throw new UnsolvableGroupException2("MinimumSum couldn't find anything.");
                if (!UpdateSum(sum))
                    throw new IllegalSumException2("Setting to minimum failed.");
            }

            return UpdateResult.changed;
        }

        public SolutionInfo Solve()
        {
            SolutionInfo result = new SolutionInfo();
            if (!SumIsKnown())
                return result;

            List<OpenCell> copyOfUnsolvedCellList = data.GetCopyOfUnsolvedCells();
            foreach (OpenCell unsolvedCell in copyOfUnsolvedCellList)
            {
                SolutionInfo cellsSolutionInfo = unsolvedCell.UpdateValueList(false);
                if (!cellsSolutionInfo.solvable)
                    return cellsSolutionInfo;
                cellsSolutionInfo.CombineWith(CheckCellValuesForViability(unsolvedCell));
//                debugLog.Write(cell.AllPossibleValues());
                if (unsolvedCell.OnlyOneSolution())
                {
                    FinalizeBothParents(unsolvedCell);
                }
                result.CombineWith(cellsSolutionInfo);
            }
            SolutionInfo newResult = result;
            while (newResult.MovedCloserToSolution())
            {
                newResult = Solve();
                result.CombineWith(newResult);
            }
            return result;
        }

        public SolutionInfo CheckAllCellsForSolvability()
        {
            List<OpenCell> copyOfUnsolvedCellList = data.GetCopyOfUnsolvedCells();
            SolutionInfo overallSolutionInfo = new SolutionInfo();
            foreach (OpenCell cell in copyOfUnsolvedCellList)
            {
                SolutionInfo cellSolutionInfo = cell.UpdateValueList(false);
                overallSolutionInfo.CombineWith(cellSolutionInfo);
                if (!overallSolutionInfo.solvable)
                    return overallSolutionInfo;
                overallSolutionInfo.CombineWith(CheckCellValuesForViability(cell));
                //if (cell.OnlyOneSolution())
                //    SetCellToSolved(cell);
            }
            return overallSolutionInfo;
        }

        private SolutionInfo CheckCellValuesForViability(OpenCell cell)
        {
//            debugLog.Write("CheckCellValuesForViability(" + cell.ToString() + ")");
            SolutionInfo solutionInfo = new SolutionInfo();
            int sumOfUnsolvedCels = SumOfAllUnsolvedCells();
            if (sumOfUnsolvedCels == 0)
            {
                solutionInfo.solvable = false;
                return solutionInfo;
            }
            List<int> possibleDigits = new List<int>(cell.GetPossibleValues());
            int initialNumberOfDigits = cell.GetPossibleValues().Count;
            List<OpenCell> copyOfUnsolvedCellList = data.GetCopyOfUnsolvedCells();
            copyOfUnsolvedCellList.Remove(cell);
            Stack<OpenCell> cellStack = new Stack<OpenCell>(copyOfUnsolvedCellList);
            foreach (int digit in possibleDigits)
            {
                if (digit <= sumOfUnsolvedCels)
                {
                    List<int> availableDigitsFromGroup = new List<int>(data.availableDigits);
                    availableDigitsFromGroup.Remove(digit);
                    if (!SumWorksForGroup(sumOfUnsolvedCels - digit, availableDigitsFromGroup, cellStack))
                    {
                        cell.DigitIsNotAPossibleValue(digit);
                    }
                }
                else
                {
                    cell.DigitIsNotAPossibleValue(digit);
                }
            }
            int finalNumberOfDigits = cell.GetPossibleValues().Count;
            solutionInfo.increaseInNumber = finalNumberOfDigits - initialNumberOfDigits;
            debugLog.Write("        Validate cell " + cell.ToString() +
                           "(start:" + initialNumberOfDigits + " end:" + finalNumberOfDigits + ")\n");
            if (finalNumberOfDigits == 0)
                solutionInfo.solvable = false;
            return solutionInfo;
        }
        
        private bool SumWorksForGroup(int sum, List<int> possibleDigits, Stack<OpenCell> cells)
        {
            // steps:
            // 1) pick a possible digit from the first item in the list.
            // 2) make sure that the digit chosen can't be used again.
            // 3) recurse using the sum that was passed in minus the chosen digit.

            //debugLog.Write("    Trying[sum:" + sum + ",cellCount:" + cells.Count() + "\n");

            if (cells.Count == 0)
                return (sum == 0) ? true : false;
            OpenCell cellToTry = cells.Pop();
            foreach (int possibleValue in cellToTry.GetPossibleValues())
            {
                if ( (possibleValue <= sum) && (possibleDigits.Contains(possibleValue)) )
                {
                    possibleDigits.Remove(possibleValue);
                    bool sumWorks = SumWorksForGroup(sum - possibleValue, possibleDigits, cells);
                    if (sumWorks)
                    {
//                        debugLog.Write("  Return true...\n");
                        cells.Push(cellToTry);
                        possibleDigits.Add(possibleValue);
                        return true;
                    }
                    else
                        possibleDigits.Add(possibleValue);
                }
            }
            cells.Push(cellToTry);            
            return false;
        }

        public List<int> GetPossibleValues()  //This is making a copy.  It would be more efficient to
        // return a readonly reference, but I don't know how to do that, yet.
        {
            return data.availableDigits.ToList();
        }

        /************************Private Methods************************/

        private void BackUpData()
        {
            foreach (OpenCell cell in data.GetCopyOfUnsolvedCells())
            {
                cell.BackUpData();
            }
            backupData = (GroupData)data.Clone();
        }

        public UpdateResult RestoreAllData()
        {
            int unusableSum = SumOfAllCells();
            data = (GroupData)backupData.Clone();
            UpdateSumCell(0);
            data.unusableSums.Add(unusableSum);
            debugLog.Write("Restored to " + GroupType() +
                           " (bad sum:" + unusableSum + ")   ");
            foreach (OpenCell cell in data.cellList)
            {
                cell.RestoreData();
                debugLog.Write(cell.ToString() + "  ");
            }
            debugLog.Write("\n");
            return UpdateResult.repaired;
        }

        public void FinalizeBothParents(OpenCell cellToFinalize)
        {
            bool cellWasFinalized = true;
            foreach (CellGroup parentGroup in cellToFinalize.GetParentGroups())
            {
                cellWasFinalized &= parentGroup.FinalizeCell(cellToFinalize);
            }
            if (cellWasFinalized)
                debugLog.Write("Finalized " + cellToFinalize.ToString() + ".\n");
        }

        private bool FinalizeCell(OpenCell cellToFinalize)
        {
            debugLog.Write("     FinalizeCell " + cellToFinalize.ToString() + "...\n");
            if (!cellToFinalize.OnlyOneSolution())
            {
                debugLog.Write("Finalize is a no-op, since there is still more than one possible solution.\n");
                return false;
            }
            data.MoveToSolvedList(cellToFinalize);
            data.availableDigits.Remove(cellToFinalize.GetValue());

            cellToFinalize.SetCellAsSolved();

            foreach (OpenCell cell in data.GetCopyOfUnsolvedCells())
            {
                if (cell == cellToFinalize)
                    continue;
                SolutionInfo solutionInfo = cell.UpdateValueList(true);
                //if (solutionInfo.MovedCloserToSolution() && cell.OnlyOneSolution())
                //    FinalizeCell(cell);
            }
            return true;
        }

        /* Here's a problem...
         * When we are rewinding solutions, we are not moving cells that found
         * their way into solved lists of both parents back out of those solved
         * lists.  I think we have to get all the cells in the puzzle back to an
         * initial state when any rewind happens.
         */

        private void UpdateAvailableValueList()
        {
            List<int> superset = new List<int>();
            foreach (OpenCell cell in data.GetCopyOfUnsolvedCells())
                superset = superset.Union(cell.GetPossibleValues()).ToList();
            data.availableDigits = (superset.Intersect(data.availableDigits)).ToList();
            if (data.availableDigits.Count == 0)
                debugLog.Write("\nEmpty availableDigits!\n\n");
        }

        //private OpenCell GetARandomUnsolvedCell()
        //{
        //    Randomizer randomInt = new Randomizer(0, data.unsolvedCellList.Count() - 1);
        //    return data.unsolvedCellList.ElementAt(randomInt.Next());
        //}

        private bool OneUnsolvedCellLeft()
        {
            return (data.NumberOfUnsolvedCells() == 1) ? true : false;
        }

        private bool SolveForLastCell()
        {
            debugLog.Write("Only one cell to try.  Solving that cell...\n");
            OpenCell cellToSolve = data.GetCopyOfUnsolvedCells().ElementAt(0);
            int randomValue = cellToSolve.GetARandomValue();
            if (randomValue == 0)
                return false;
            int sum = SumOfAllSolvedCells() + randomValue;
            if (data.unusableSums.Contains(sum))
            {
                cellToSolve.DigitIsNotAPossibleValue(randomValue);
                return SolveForLastCell();
            }
            cellToSolve.SetToAValue(randomValue);
            FinalizeBothParents(cellToSolve);
            return UpdateSum(sum);  // Need to deal with a failure here...
        }

        abstract protected bool UpdateSumCell(int value);
        abstract public int SumOfAllCells();
        abstract public string GroupType();
        
        public bool SumIsKnown()
        {
            return (SumOfAllCells() == 0)? false : true;
        }

        public bool UpdateSum(int newSum)
        {
            debugLog.Write("  !!!!!               Attempting to update sum to " + newSum + "...\n");
            if (newSum == 0)
            {
                debugLog.Write("Yikes!  Got an update for a 0 sum!\n");
                PrintAllCells();
                return false;
            }
            if (data.unusableSums.Contains(newSum))
            {
                debugLog.Write("Tried to use one of the nasty unusable sums.\n");
                return false;
            }

            if (newSum == SumOfAllSolvedCells())
                PrintAllCells();

            UpdateSumCell(newSum);
            debugLog.Write("  Successful.\n");
            return true;
        }

        private bool MaximumSum(out int sum)
        {
            if (SumIsKnown())
            {
                sum = SumOfAllCells();
                return true;
            }

            int sumOfAllSolvedCells = SumOfAllSolvedCells();

            int greatestPossibleSumOfUnsolvedCells = 
                CellGroupToolkit.GreatestPossibleSum(data.GetCopyOfUnsolvedCells());
            if (greatestPossibleSumOfUnsolvedCells == 0)
            {
                sum = greatestPossibleSumOfUnsolvedCells;
                return false;
            }
            sum = greatestPossibleSumOfUnsolvedCells + sumOfAllSolvedCells;
            sum = GetTheGreatestValidSum(sum);
            if (sum < CellGroupToolkit.LeastPossibleSum(data.GetCopyOfUnsolvedCells()) + sumOfAllSolvedCells)
                return false;

            return true;
        }

        private int GetTheGreatestValidSum(int sum)
        {
            while (data.unusableSums.Contains(sum))
                --sum;
            return sum;
        }

        private bool MinimumSum(out int sum)
        {
            if (SumIsKnown())
            {
                sum = SumOfAllCells();
                return true;
            }

            int leastPossibleSumOfUnsolvedCells =
                CellGroupToolkit.LeastPossibleSum(data.GetCopyOfUnsolvedCells());

            if (leastPossibleSumOfUnsolvedCells == 0)
            {
                sum = leastPossibleSumOfUnsolvedCells;
                return false;
            }

            int sumOfAllSolvedCells = SumOfAllSolvedCells();
            sum = leastPossibleSumOfUnsolvedCells + sumOfAllSolvedCells;
            sum = GetTheLeastValidSum(sum);
            if (sum > CellGroupToolkit.GreatestPossibleSum(data.GetCopyOfUnsolvedCells())
                                                             + sumOfAllSolvedCells)
                return false;
            return true;
        }

        private int GetTheLeastValidSum(int sum)
        {
            while (data.unusableSums.Contains(sum))            
                ++sum;            
            return sum;
        }

        private int SumOfAllSolvedCells()
        {
            int sum = 0;
            foreach (OpenCell cell in data.GetCopyOfSolvedCells())
            {
                sum += cell.GetValue();
            }
            return sum;
        }

        private int SumOfAllUnsolvedCells()
        {                                    // This needs to change...
            if (!SumIsKnown())
                return 0;
            int sumOfUnsolvedCells = SumOfAllCells();
            foreach (OpenCell cell in data.GetCopyOfSolvedCells())
                sumOfUnsolvedCells -= cell.GetValue();
            return sumOfUnsolvedCells;
        }

        public void RevertToInitialState()
        {
            RevertToUnsolvedState();
            data.unusableSums = new List<int>();
            UpdateSumCell(0);
        }

        public void RevertToUnsolvedState()
        {
            data.RevertToUnsolvedState();
            PrintAllCells();
        }

        public void PrintAllCells()
        {
            debugLog.Write(ToString());
        }

        public override string ToString()
        {
            string returnStr = GroupType() + " - Sum of unsolved cells:" + SumOfAllUnsolvedCells();
            returnStr += "     Unsolved: ";
            foreach (OpenCell cell in data.GetCopyOfUnsolvedCells())
            {
                returnStr += cell.ToString() + "  ";
            }
            returnStr += "     Solved: ";
            foreach (OpenCell cell in data.GetCopyOfSolvedCells())
            {
                returnStr += cell.ToString() + "  ";
            }
            returnStr += "\n";
            return returnStr;
        }

        private void PrintPossibleValues()
        {
            debugLog.Write("[");
            foreach (int value in data.availableDigits)
            {
                debugLog.Write(value + " ");
            }
            debugLog.Write("]");
        }

        private static bool MaximumForRows(out int sumOfAllRows)
        {
            sumOfAllRows = 0;
            foreach (CellGroup group in rowList)
            {
                int sum = 0;
                bool successful = group.MaximumSum(out sum);
                if (!successful)
                {
                    CrossSumsForm.debugLog.Write("MaximumForRows can't resolve!");
                    return false;
                }
                sumOfAllRows += sum;
            }
            return true;
        }

        private static bool MaximumForColumns(out int sumOfAllColumns)
        {
            sumOfAllColumns = 0;
            foreach (CellGroup group in columnList)
            {
                int sum = 0;
                bool successful = group.MaximumSum(out sum);
                if (!successful)
                {
                    CrossSumsForm.debugLog.Write("MaximumForColumns can't resolve!");
                    return false;
                }
                sumOfAllColumns += sum;
            }
            return true;
        }

        private static bool MinimumForRows(out int sumOfAllRows)
        {
            sumOfAllRows = 0;
            foreach (CellGroup group in rowList)
            {
                int sum = 0;
                bool successful = group.MinimumSum(out sum);
                if (!successful)
                {
                    CrossSumsForm.debugLog.Write("MinimumForRows can't resolve!");
                    return false;
                }
                sumOfAllRows += sum;
            }
            return true;
        }

        private static bool MinimumForColumns(out int sumOfAllColumns)
        {
            sumOfAllColumns = 0;
            foreach (CellGroup group in columnList)
            {
                int sum = 0;
                bool successful = group.MinimumSum(out sum);
                if (!successful)
                {
                    CrossSumsForm.debugLog.Write("MinimumForColumns can't resolve!");
                    return false;
                }
                sumOfAllColumns += sum;
            }
            return true;
        }

        private GroupData data;
        private GroupData backupData;
        protected SumCell sumCell;
        protected DebugLog debugLog;

        private class GroupData:ICloneable
        {
            public GroupData(List<OpenCell> cellListToCopy)
            {
                cellList = new List<OpenCell>(cellListToCopy);
                unusableSums = new List<int>();
                InitializeData();
            }

            public void RevertToUnsolvedState()
            {
                InitializeData();
            }

            private void InitializeData()
            {
                availableDigits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                solvedList = new List<OpenCell>();
                unsolvedList = new List<OpenCell>(cellList);
            }

            public List<OpenCell> GetCopyOfUnsolvedCells()
            {
                return new List<OpenCell>(unsolvedList);
            }

            public int NumberOfUnsolvedCells()
            {
                return unsolvedList.Count;
            }

            public List<OpenCell> GetCopyOfSolvedCells()
            {
                return new List<OpenCell>(solvedList);
            }

            public void MoveToSolvedList(OpenCell cell)
            {
                if (!unsolvedList.Remove(cell))
                {
                    return;
                }
                solvedList.Add(cell);
            }
            public object Clone()
            {
                GroupData returnData = new GroupData(cellList);
                returnData.availableDigits = new List<int>(availableDigits);
                returnData.unusableSums = new List<int>(unusableSums);
                returnData.unsolvedList = new List<OpenCell>(unsolvedList);
                returnData.solvedList = new List<OpenCell>(solvedList);
                return returnData;
            }

            public readonly List<OpenCell> cellList;
            List<OpenCell> unsolvedList;
            List<OpenCell> solvedList;
            public List<int> availableDigits;
            public List<int> unusableSums;
        }
    }

    class ColumnCellGroup : CellGroup
    {
        public ColumnCellGroup(SumCell sumCell, List<OpenCell> cellList, DebugLog log)
            : base(sumCell, cellList, log)
        {
        }

        ~ ColumnCellGroup()
        {
            CellGroup.columnList.Remove(this);
        }

        override protected bool UpdateSumCell(int value)
        {
            debugLog.Write("Updating column to " + value.ToString() + "\n");
            return sumCell.SetColumnSum(value);
        }

        public override int SumOfAllCells()
        {
            return sumCell.getColumnSum();
        }

        override public string GroupType() { return "Column"; }
    }

    class RowCellGroup : CellGroup
    {
        public RowCellGroup(SumCell sumCell, List<OpenCell> cellList, DebugLog log)
            : base(sumCell, cellList, log)
        {
        }

        ~ RowCellGroup()
        {
            CellGroup.rowList.Remove(this);
        }

        override protected bool UpdateSumCell(int value)
        {
            debugLog.Write("Updating row to " + value.ToString() + "\n");
            return sumCell.SetRowSum(value);
        }

        public override int SumOfAllCells()
        {
            return sumCell.GetRowSum();
        }

        override public string GroupType() { return "Row"; }
    }
}
