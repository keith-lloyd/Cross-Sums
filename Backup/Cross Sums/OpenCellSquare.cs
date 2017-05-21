using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    /***********************************************************************
     * OpenCellSquareMatrix
     ***********************************************************************
     * The reason this class is needed, is that n x n squares of open cells are
     * unsolvable if every cell winds up with the same n possible solutions.
     * This class checks for that situation, so it can be protected against.
     ***********************************************************************/
    class OpenCellSquare
    {
        public OpenCellSquare(Queue<OpenCell> cells)
        {
            cellQueue = cells;
            sizeOfSquare = (int)Math.Sqrt(cellQueue.Count);
            isWellFormed = ((sizeOfSquare > 0) &&
                            (double)sizeOfSquare == Math.Sqrt(cellQueue.Count)) ? true : false;
        }

        public bool IsSolvable()
        {
            if (!isWellFormed)
                return false;

            List<int> possibleValues = cellQueue.First().GetPossibleValues();
            if (possibleValues.Count != sizeOfSquare)
                return true;
            foreach (OpenCell cell in cellQueue)
            {
                if (possibleValues != cell.GetPossibleValues())
                    return true;
            }
            return false;
        }

        public bool NoLongerAtRiskOfUnsolvability()
        {
            if (!isWellFormed)
                return true;
            foreach (OpenCell cell in cellQueue)
            {
                if (cell.GetPossibleValues().Count < sizeOfSquare)
                    return true;
            }

            return false;
        }

        Queue<OpenCell> cellQueue;
        bool isWellFormed;
        int sizeOfSquare;
    }
    
}
