using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    public class SolutionInfo
    {
        public bool solvable = true;
        public int increaseInNumber = 0;
        public bool MovedCloserToSolution()
        {
            return (solvable && (increaseInNumber < 0)) ? true : false;
        }
        public void CombineWith (SolutionInfo infoToCombine)
        {
            if (solvable && infoToCombine.solvable)
                solvable = true;
            else
                solvable = false;
            if (increaseInNumber > infoToCombine.increaseInNumber)
                increaseInNumber = infoToCombine.increaseInNumber;
        }
    }

}
