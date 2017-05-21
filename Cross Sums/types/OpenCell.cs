using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Cross_Sums.types;

namespace Cross_Sums
{
    public enum Tendency { low, medium, high };

    public class OpenCell : TextBox, ICell
    {
        public OpenCell (Coordinates coord, CrossSumsMatrix matrix)
        {
            myCoordinates = new Coordinates(coord.X, coord.Y);
            this.matrix = matrix;
            this.debugLog = matrix.debugLog;
            CreateCellForm(myCoordinates.X, myCoordinates.Y);
            AcceptsReturn = false;
            Multiline = false;
            MaxLength = 1;
            parentGroups = new Queue<CellGroup>();
            Initialize();
       }

        public void Initialize ()
        {
            cellSolutionInfo = new CellSolutionInfo();
        }

        public void AddParentGroup(CellGroup parentGroup)
        {
            parentGroups.Enqueue(parentGroup);
        }

        public bool ValueIsInPossibleValueList(int value)
        {
            return (cellSolutionInfo.Contains(value)) ? true : false;
        }

        public List<int> GetPossibleValues()
        {
            return cellSolutionInfo.GetPossibleValues();
        }

        public void DigitIsNotAPossibleValue(int digit)
        {
            //bool startedWithMoreThanOneSolution = MoreThanOneSolution(); 
            cellSolutionInfo.DigitIsNotAPossibleValue(digit);
            //if (startedWithMoreThanOneSolution && OnlyOneSolution())
            //{
            //    SetCellAsSolved();
            //}
        }

        /***************************************************************************
         * UpdateValueList()
         * tests the list of solutions, and removes the ones that do not work.
         * returns the increase in solutions (i.e. a negative number is good.)
         **************************************************************************/
        public SolutionInfo UpdateValueList(bool debugStatementsOn)
        {
            if (debugStatementsOn)
                debugLog.Write("Starting to UpdateValueList cellToTry (" + ToString() + "->");
            SolutionInfo returnInfo = new SolutionInfo();
            int initialNumOfSolutions = cellSolutionInfo.NumberOfPossibleDigits();
            returnInfo.CombineWith(MergeValueListWithParentGroups());

            int updatedNumberOfSolutions = cellSolutionInfo.NumberOfPossibleDigits();
            returnInfo.increaseInNumber = updatedNumberOfSolutions - initialNumOfSolutions;

            if (debugStatementsOn)
                debugLog.Write(ToString() + ")\n");
            return returnInfo;
        }

        public Tendency TendencyOfCell()
        {
            if (cellSolutionInfo.GetPossibleValues().Count == 0)
            {
                debugLog.Write("");
                debugLog.Write("ERROR /_/~/_/~/_/~/_/ TendencyOfCell encountered an empty solution info list!!!");
                debugLog.Write("");
                return Tendency.medium;
            }
            double average = cellSolutionInfo.GetPossibleValues().Average();
            if (average <= 4)
                return Tendency.low;
            if (average >= 6)
                return Tendency.high;
            else
                return Tendency.medium;
        }

        private SolutionInfo MergeValueListWithParentGroups()
        {
            SolutionInfo returnInfo = new SolutionInfo();
            int originalCount = cellSolutionInfo.NumberOfPossibleDigits();
            foreach (CellGroup parentGroup in parentGroups)
            {
                cellSolutionInfo.IntersectWith(parentGroup.GetPossibleValues());
   // If this list shrinks, we have moved closer to a solution.  The returnInfo does not reflect this.
            }

            if (NoSolutions())
                returnInfo.solvable = false;
            if ((originalCount > 1))
            {
                //SetUniqueValueOnDisplay();
            }

            int newCount = cellSolutionInfo.NumberOfPossibleDigits(); 
            returnInfo.increaseInNumber = originalCount - newCount;
            return returnInfo;
        }

        /************************************************************
         * GetValue
         ************************************************************
         * Returns the solution if there is only one solution left.
         * Otherwise, returns 0.
         ************************************************************/
        public int GetValue()
        {
            if (OnlyOneSolution())
                return cellSolutionInfo.GetPossibleValues()[0];
            return 0;
        }

        public bool NoSolutions()
        {
            return (cellSolutionInfo.NumberOfPossibleDigits() == 0) ? true : false;
        }

        public bool OnlyOneSolution()
        {
            return (cellSolutionInfo.NumberOfPossibleDigits() == 1) ? true : false;
        }

        public bool MoreThanOneSolution()
        {
            return (cellSolutionInfo.NumberOfPossibleDigits() > 1) ? true : false;
        }

        public void DrawYourself(CrossSumsForm form)
        {
            form.AddOpenCell(this);
        }

        public void BackUpData()
        {
            cellSolutionInfo.BackUp(); 
        }

        public void RestoreData()
        {
            cellSolutionInfo.Restore();
            Text = "?";
        }

        public Queue<CellGroup> GetParentGroups()
        {
            return parentGroups;
        }

        /********************************
         * SetUniqueValueOnDisplay()
         * Assumes only one valid group, displays that group in its cell.
         ********************************/
        public void SetCellAsSolved()
        {
            Text = cellSolutionInfo.GetPossibleValues().ElementAt(0).ToString();
        }

        public int GetARandomValue()
        {
            return Randomizer.RandomSelectFrom(cellSolutionInfo.GetPossibleValues());
        }

        public void SetToAValue(int value)
        {
            cellSolutionInfo.ForceToAValue(value);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (!IsAnIntegerBetweenOneAndNine(Text))
            {
                if (Text != "?")
                    Text = "";
            }
            base.OnTextChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                matrix.MoveFocusUp(myCoordinates);
            if (e.KeyCode == Keys.Down)
                matrix.MoveFocusDown(myCoordinates);
            if (e.KeyCode == Keys.Right)
                matrix.MoveFocusRight(myCoordinates);
            if (e.KeyCode == Keys.Left)
                matrix.MoveFocusLeft(myCoordinates);
        }

        public bool AssumeFocus()
        {
          Focus();
          SelectAll();
          return true;
        }

        public override string ToString()
        {
            string returnStr = "(" + myCoordinates.X + "," + myCoordinates.Y + ") < ";
            foreach (int value in cellSolutionInfo.GetPossibleValues())
            {
                returnStr += value + " ";
            }
            returnStr += ">  ";
            return returnStr;
        }

        public string AllPossibleValues()
        {
            string returnString = "  New poss. values: ";
            if (GetPossibleValues().Count() == 0)
                returnString += "[empty]\n";
            else
            {
                foreach (int i in cellSolutionInfo.GetPossibleValues())
                    returnString += i + " ";
            }
            return returnString;
        }

        static private bool IsAnIntegerBetweenOneAndNine(string str)
        {
            bool stringIsLessThanOne = (str.CompareTo("1") == -1);
            bool stringIsGreaterThanNine = (str.CompareTo("9") == 1);
            if (stringIsLessThanOne || stringIsGreaterThanNine)
            {
                return false;
            }
            return true;
        }

        private void CreateCellForm(int x, int y)
        {
            int upperLeftX = (x + 1) * constants.cellWidth;
            int upperLeftY = (y + 1) * constants.cellWidth;
            Location = new Point(upperLeftX, upperLeftY);
            Size = new Size(constants.cellWidth, constants.cellWidth);

            Font = new Font(new FontFamily("Arial"), 20);
            TextAlign = HorizontalAlignment.Center;
            BackColor = System.Drawing.Color.Honeydew;
            BorderStyle = BorderStyle.FixedSingle;
        }

        private DebugLog debugLog;
        private CrossSumsMatrix matrix;
        private CellSolutionInfo cellSolutionInfo;
        private readonly Coordinates myCoordinates;
        public Queue<CellGroup> parentGroups;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShortcutsEnabled = false;
            this.ResumeLayout(false);

        }
    }
}