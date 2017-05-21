using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Cross_Sums
{
    public class SumCell : ICell
    {
        public SumCell(int x, int y)
        {
            this.x = x;
            this.y = y;
            rowSum = 0;
            columnSum = 0;
            rowSumIllegalValues = new List<int>();
            columnSumIllegalValues = new List<int>();
        }

        public void DrawYourself(CrossSumsForm form)
        {
            if ( (rowSum == 0) && (columnSum == 0) )
                form.DrawBlackBox(this);
            else
                form.DrawSumsCell(this, rowSum, columnSum);
        }

        public bool AssumeFocus()
        {
            return false;
        }

        public bool SetRowSum(int newValue)
        {
            if (!rowSumIllegalValues.Contains(newValue))
            {
                rowSum = newValue;
                return true;
            }
            return false;
        }

        public bool SetColumnSum(int newValue)
        {
            if (!columnSumIllegalValues.Contains(newValue))
            {
                columnSum = newValue;
                return true;
            }
            return false;
        }

        public int GetRowSum()
        {
            return rowSum;
        }

        public int getColumnSum()
        {
            return columnSum;
        }

        public readonly int x;
        public readonly int y;
        private int rowSum;
        private List<int> rowSumIllegalValues;
        private int columnSum;
        private List<int> columnSumIllegalValues;
    }

}
