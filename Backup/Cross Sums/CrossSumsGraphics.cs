using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    public class CrossSumsGraphics
    {
        public CrossSumsGraphics(CrossSumsForm form)
        {
            this.form = form;
        }

        public void Write(string strToWrite)
        {
            form.Write(strToWrite);
        }

        public void DrawYourself()
        {
            for (int x = 0; x < constants.dimensions; ++x)
            {
                for (int y = 0; y < constants.dimensions; ++y)
                {
                    cellMatrix[x, y].DrawYourself(form);
                }
            }
        }

        public void MoveFocusDown(int currentX, int currentY)
        {
            if (++currentY < constants.dimensions)
                cellMatrix[currentX, currentY].AssumeFocus();
        }

        public void MoveFocusUp(int currentX, int currentY)
        {
            if (--currentY >= 0)
                cellMatrix[currentX, currentY].AssumeFocus();
        }

        public void MoveFocusRight(int currentX, int currentY)
        {
            if (++currentX < constants.dimensions)
                cellMatrix[currentX, currentY].AssumeFocus();
        }

        public void MoveFocusLeft(int currentX, int currentY)
        {
            if (--currentX >= 0)
                cellMatrix[currentX, currentY].AssumeFocus();
        }

        protected CrossSumsForm form;
        protected ICell[,] cellMatrix;
    }
}
