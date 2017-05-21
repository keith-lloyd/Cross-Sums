using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cross_Sums.types;

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

        public void MoveFocusDown(Coordinates currentCoordinates)
        {
            if (currentCoordinates.Y + 1 < constants.dimensions)
                cellMatrix[currentCoordinates.X, currentCoordinates.Y + 1].AssumeFocus();
        }

        public void MoveFocusUp(Coordinates currentCoordinates)
        {
            if (currentCoordinates.Y - 1 >= 0)
                cellMatrix[currentCoordinates.X, currentCoordinates.Y - 1].AssumeFocus();
        }

        public void MoveFocusRight(Coordinates currentCoordinates)
        {
            if (currentCoordinates.X + 1 < constants.dimensions)
                cellMatrix[currentCoordinates.X + 1, currentCoordinates.Y].AssumeFocus();
        }

        public void MoveFocusLeft(Coordinates currentCoordinates)
        {
            if (currentCoordinates.X - 1 >= 0)
                cellMatrix[currentCoordinates.X - 1, currentCoordinates.Y].AssumeFocus();
        }

        protected CrossSumsForm form;
        protected ICell[,] cellMatrix;
    }
}
