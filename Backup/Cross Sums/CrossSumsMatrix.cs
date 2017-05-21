using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cross_Sums.types;

namespace Cross_Sums
{
   public class CrossSumsMatrix : CrossSumsGraphics
   {
       public CrossSumsMatrix (CrossSumsForm form) : base(form)
       {
           debugLog = CrossSumsForm.debugLog;
           bool solvablePuzzleCreated = false;
           try
           {
               while (!solvablePuzzleCreated)   // change "if" to "while" to retry indefinitely.
               {
                   Initialize();
                   solvablePuzzleCreated = CreateThePuzzle();
               }
               if (solvablePuzzleCreated)
               {
                   foreach (OpenCell cell in openCells)
                   {
                       cell.Text = "0";
                   }
               }
           }
           catch (Exception e)
           {
               debugLog.Write("\n\n\nSomething Really Bad Happened!\n\n\n");
               debugLog.Write(e.Message);
           }
           debugLog.Write("\nAll done!\n");
       }

       private void Initialize()
       {
           cellGroupsWithSums = new Stack<CellGroup>();
           cellGroupsWithoutSums = new Stack<CellGroup>();
           cellMatrix = new ICell[constants.dimensions, constants.dimensions];
           openCells = new Stack<OpenCell>();
           CellGroup.columnList = new List<CellGroup>();
           CellGroup.rowList = new List<CellGroup>();
           CreateThePattern();
       }

       private bool CreateASymmetricOpenCellFoursome (int row, int column)
       {
           bool successful = true;
           if (!CreateTwoSymmetricOpenCells(row, column))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(row+1, column))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(row, column+1))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(row+1, column+1))
               successful = false;
           return successful;
       }

       public bool CreateTwoSymmetricOpenCells(int row, int column)
       {
           if ((row < 1) || (column < 1))
               return false;
           if ((row >= constants.dimensions) || (column >= constants.dimensions))
               return false;
           ICell cell = cellMatrix[row, column];
           if (cell == null)
           {
               debugLog.Write("creating an open cell at "
                              + row.ToString() + column.ToString() + "\n");
               OpenCell newCell = new OpenCell(row, column, this);
               cellMatrix[row, column] = newCell;
               openCells.Push(newCell);
           }
           int symmRow = constants.dimensions - row;
           int symmColumn = constants.dimensions - column;
           ICell symmetricCell = cellMatrix[symmRow, symmColumn];
           if (symmetricCell == null)
           {
               debugLog.Write("creating an open cell at "
                              + symmRow.ToString() + symmColumn.ToString() + "\n");
               OpenCell newCell = new OpenCell(symmRow, symmColumn, this);
               cellMatrix[symmRow, symmColumn] = newCell;
               openCells.Push(newCell);
           }
           return true;
       }

       public void CreateBlackCells()
       {
           for (int y = 0; y < constants.dimensions; ++y)
           {
               for (int x = 0; x < constants.dimensions; ++x)
               {
                   if (cellMatrix[x, y] == null)
                   {
                       cellMatrix[x, y] = new SumCell(x, y);
                   }
               }
           }
       }

       private void SetOpenCellTabOrder()
       {
           int componentTabOrder = 0;
           for (int y = 1; y < constants.dimensions; ++y)
           {
               for (int x = 1; x < constants.dimensions; ++x)
               {
                   if (CellIsOpen(x,y))
                   {
                       ((OpenCell)cellMatrix[x, y]).TabIndex = componentTabOrder++;
                   }
               }
           }
       }

       /********************************************************
        *                CreateAllColumnGroups
        ********************************************************
        * When this method is called, the pattern has been
        * determined.  This method puts the open copyOfUnsolvedCellList into
        * their proper row and column groups.
        *******************************************************/
       private void CreateAllColumnGroups()
       {
           Coordinates coordinates = new Coordinates(0, 0);
           while(FindNextOpenColumn(coordinates))
           {
               CreateAColumnGroup(coordinates);
               if (coordinates.Y == constants.dimensions)
                   --coordinates.Y;
           }
           return;
       }

       private void CreateAColumnGroup(Coordinates coordinates)
       {
           Queue<OpenCell> cellQueue = new Queue<OpenCell>();
           SumCell columnSumCell = (SumCell)cellMatrix[coordinates.X, coordinates.Y - 1];
           while ((coordinates.Y < constants.dimensions) && CellIsOpen(coordinates))
           {
               OpenCell cellToAdd = (OpenCell)cellMatrix[coordinates.X, coordinates.Y];
               cellQueue.Enqueue(cellToAdd);
               coordinates.Y++;
           }
           CellGroup newGroup = new ColumnCellGroup(columnSumCell, new List<OpenCell>(cellQueue), debugLog);
           CellGroup.columnList.Add(newGroup);
           cellGroupsWithoutSums.Push(newGroup);
       }

       private bool FindNextOpenColumn(Coordinates coords)
       {
           // Go through any black copyOfUnsolvedCellList, find first open cellToTry.
           bool currentlyInAnOpenColumn = false;
           if (CellIsOpen(coords))
               currentlyInAnOpenColumn = true;
           for (int x = coords.X; x < constants.dimensions; ++x)
           {
               for (int y = coords.Y; y < constants.dimensions; ++y)
               {
                   if (CellIsOpen(x,y))
                   {
                       if (!currentlyInAnOpenColumn)
                       {
                           coords.X = x;
                           coords.Y = y;
                           return true;
                       }
                   }
                   else
                       currentlyInAnOpenColumn = false;
               }
               coords.Y = 0;
           }
           coords = new Coordinates(0, 0);
           return false;
       }

       private void CreateAllRowGroups()
       {
           Coordinates coordinates = new Coordinates(0, 0);
           while (FindNextOpenRow(coordinates))
           {
               CreateARowGroup(coordinates);
           }
           return;
       }

       private void CreateARowGroup(Coordinates coordinates)
       {
           Queue<OpenCell> cellQueue = new Queue<OpenCell>();
           SumCell rowSumCell = (SumCell)cellMatrix[coordinates.X-1, coordinates.Y];
           while ((coordinates.X < constants.dimensions) && CellIsOpen(coordinates))
           {
               OpenCell cellToAdd = (OpenCell)cellMatrix[coordinates.X, coordinates.Y];
               cellQueue.Enqueue(cellToAdd);
               coordinates.X++;
           }
           if (coordinates.X == constants.dimensions)
               --coordinates.X;
           CellGroup newGroup = new RowCellGroup(rowSumCell, new List<OpenCell>(cellQueue), debugLog);
           CellGroup.rowList.Add(newGroup);
           cellGroupsWithoutSums.Push(newGroup);
       }

       private bool FindNextOpenRow(Coordinates coords)
       {
           // Go through any black copyOfUnsolvedCellList, find first open cellToTry.
           bool currentlyInAnOpenRow = false;
           if (CellIsOpen(coords))
               currentlyInAnOpenRow = true;
           for (int y = coords.Y; y < constants.dimensions; ++y)
           {
               for (int x = coords.X; x < constants.dimensions; ++x)
               {
                   if (CellIsOpen(x, y))
                   {
                       if (!currentlyInAnOpenRow)
                       {
                           coords.X = x;
                           coords.Y = y;
                           return true;
                       }
                   }
                   else
                       currentlyInAnOpenRow = false;
               }
               coords.X = 0;
           }
           coords = new Coordinates(0, 0);
           return false;
       }

       private bool CellIsOpen(Coordinates coords)
       {
           if (cellMatrix[coords.X, coords.Y] is OpenCell)
               return true;
           return false;
       }

       private bool CellIsOpen(int x, int y)
       {
           if (cellMatrix[x, y].GetType() == typeof(OpenCell))
               return true;
           return false;
       }

       private void CreateThePattern()
       {
           CreateASymmetricOpenCellFoursome(1, 1);   // and (5,5)
           CreateASymmetricOpenCellFoursome(1, 4);   // and (4,3)
           CreateASymmetricOpenCellFoursome(3, 2);   // and (3,4)

           CreateBlackCells();
           SetOpenCellTabOrder();
       }

       public bool CreateThePuzzle()
       {
           bool solvablePuzzleCreated = true;
//           int maxNumberOfLoops = 200;
           CreateAllColumnGroups();
           CreateAllRowGroups();
           RandomlyOrderGroupsWithoutSums();
           //FindAllSquares();

           // 1. Select a random cellToTry, and pick a valid solution for it.  (Done here in CSM)
           // 2. Restrict possible solutions to all copyOfUnsolvedCellList in both parents.  (Done in the chosen parent group.)
           // 3. Remove completely solved groups from the puzzle.  (Done in the chosen parent group.)
           // 4. Are all the copyOfUnsolvedCellList solved?  No: go to 1,  Yes go to 5.  (Done here in CSM)
           // 5. Verify the solution?  Possibly not...

//           int loopCount = 0;
           UpdateResult result = UpdateResult.changed;
           TimeSpan span = new TimeSpan();
           double mostrecentTime = span.TotalSeconds;
           
           while (result != UpdateResult.unchanged)
           {
               result = FindSolutionsForCells();
               if (result == UpdateResult.repaired)  // "while", not "if"?
               {
                 try
                 {
                   SolvePuzzle();  // If changed to while, got to update result here.
                 }
                 catch (UnsolvableGroupException2 exception)
                 {
                   debugLog.Write("The current group cannot be solved.  Reverting last group.\n");
                   debugLog.Write(exception.Message);
                   solvablePuzzleCreated = false;
                   break;
                 }
               }
//               ++loopCount;
               debugLog.Write(CellGroup.PrintAllColumnSums());
               debugLog.Write(CellGroup.PrintAllRowSums());
               debugLog.Write("\n--------\nNumber of cell groups without sums left:"
                              + cellGroupsWithoutSums.Count + "    " + result + "\n--------\n");
               //if (loopCount > maxNumberOfLoops)
               //{
               //  solvablePuzzleCreated = false;
               //  break;
               //}
               double currentTime = span.TotalSeconds;
               double timeSpent = currentTime - mostrecentTime;
               if (timeSpent > 2)
               {
                   debugLog.Write("\n\n+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
                   debugLog.Write("                                  Total Time spent: " + timeSpent + "\n");
                   debugLog.Write("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n\n\n");
               }
           }
           if (!solvablePuzzleCreated)
           {
               debugLog.Write("Had a problem.  I'll retry from the very beginning.\n");
               solvablePuzzleCreated = false;
           }
           return solvablePuzzleCreated;
       }

       /*************************************************************************************
        *                               FindSolutionsForCells
        *------------------------------------------------------------------------------------
        *  This method takes a cellToTry from the matrix, and examines the cellToTry's corresponding
        *  parent groups (i.e. the collumn and row to which the cellToTry belongs.)  One of
        *  the parent groups is selected to be maximized, and the other is minimized.  The
        *  digit for the cellToTry is then the only possible digit at the interesection of the two
        *
        * Outstanding problems:
        *   A sum can be set for a group that is impossible to achieve because possible values
        *   for copyOfUnsolvedCellList have not been checked.
        *
        *************************************************************************************/
       private UpdateResult FindSolutionsForCells()
       {
           debugLog.Write("\n________________________________\n\n");
           UpdateResult setSumResult = UpdateResult.unchanged;
           UpdateResult solvePuzzleResult = UpdateResult.unchanged;
           if (cellGroupsWithoutSums.Count != 0)
           {
               CellGroup groupToSolve = cellGroupsWithoutSums.Peek();
               try
               {
                 setSumResult = groupToSolve.SetTheSum();
                 // write a new method that checks for groups that could swapcells.  If a pair is found,
                 // throw an IllegalSumException2.
                 MoveNextGroupToSummedQueue();
               }
               catch (IllegalSumException2 exception)
               {
                   debugLog.Write("There was a problem finding a sum.  Restoring group.\n");
                   debugLog.Write(exception.Message);
                   setSumResult = groupToSolve.RestoreAllData();
                   CleanOutAllCellValues();
               }

               // The following has been removed to a higher level.  now we just let it go,
               // and recreate the puzzle from scratch.

               //catch (UnsolvableGroupException2 exception)
               //{
               //    debugLog.Write("The current group cannot be solved.  Reverting last group.\n");
               //    debugLog.Write(exception.Message);
               //    setSumResult = groupToSolve.RestoreAllData();  // No... Something more is needed.  Complete reset.
               //    setSumResult = RewindLastSummedGroup();
               //}

               debugLog.Write("=================>SetTheSum returned success:" + setSumResult + "\n");
               solvePuzzleResult = SolvePuzzle();
           }
           else
           {
               solvePuzzleResult = SolvePuzzle();
               debugLog.Write("=================>SolvePuzzle returned success:" + solvePuzzleResult + "\n");
               foreach (CellGroup group in cellGroupsWithSums)
               {
                   if (group.CannotBeSolved())
                   {
                       debugLog.Write("  SolvePuzzle found an unsolvable group.\n");
                       debugLog.Write("    I'm not going to raise an error, I'll just handle it here.\n");
                       RewindLastSummedGroup();
                       return UpdateResult.repaired;
                   }
               }
           }

           if ((setSumResult == UpdateResult.repaired) ||
               (solvePuzzleResult == UpdateResult.repaired))
               return UpdateResult.repaired;

           if ((setSumResult == UpdateResult.changed) ||
               (solvePuzzleResult == UpdateResult.changed))
               return UpdateResult.changed;
           
           return UpdateResult.unchanged;
       }

       private void MoveNextGroupToSummedQueue()
       {
           CellGroup groupToSolve = cellGroupsWithoutSums.Pop();
           cellGroupsWithSums.Push(groupToSolve);
       }

       private UpdateResult RewindLastSummedGroup()
       {
           debugLog.Write("Cleaning out all groups.\n");
           CellGroup groupToMove = cellGroupsWithSums.Pop();
           CleanOutAllCellValues();
           groupToMove.RestoreAllData();
           cellGroupsWithoutSums.Push(groupToMove);
           return UpdateResult.repaired;
       }

       private UpdateResult SolvePuzzle()
       {
           UpdateResult updateResult = UpdateResult.unchanged;
           debugLog.Write("SolvePuzzle()\n");
           bool movedCloserToSolution = true;
           try
           {
               while (movedCloserToSolution)
               {
                   movedCloserToSolution = false;
                   //               SyncAllOpenCells();
                   foreach (CellGroup cellGroup in cellGroupsWithSums)
                   {
                       SolutionInfo result = cellGroup.Solve();
                       if (!result.solvable)
                       {
                           if (cellGroupsWithSums.Count == 0)
                               debugLog.Write("No more cell groups with sums left!\n");
                           throw (new IllegalSumException2("Solve attempt failed.  Group: " + cellGroup.ToString()));
                       }
                       if (result.MovedCloserToSolution())
                       {
                           movedCloserToSolution = true;
                           updateResult = UpdateResult.changed;
                       }
                   }
               }
           }
           catch (types.IllegalSumException2 exception)
           {
               debugLog.Write(">>>>>>>>>>>> Caught an unsolvable exception, after the group set a sum.\n");
               debugLog.Write("  The error is:  " + exception.Message);
               updateResult = RewindLastSummedGroup();
           }
           MoveSolvedGroups();
           return updateResult;
       }


       /**************************************************************************
        *   RemovedSolvedGroups gets rid of the solved groups so that we don't
        *   keep cycling through them needlessly.  It can probably be done more
        *   efficiently using the list RemoveAll method, but I need to learn more
        *   about Predicates first.
        **************************************************************************/
       private void MoveSolvedGroups()
       {
           Stack<CellGroup> newCellGroupsWithoutSums = new Stack<CellGroup>();
           foreach (CellGroup group in cellGroupsWithoutSums)
           {
               if (!group.SumIsKnown())
                   newCellGroupsWithoutSums.Push(group);
               else
                   cellGroupsWithSums.Push(group);
           }
           cellGroupsWithoutSums = newCellGroupsWithoutSums;
       }

       private void RandomlyOrderGroupsWithoutSums()
       {
           Stack<CellGroup> newCellGroups = new Stack<CellGroup>();
           Randomizer randomCellGroupIndex = new Randomizer(0, cellGroupsWithoutSums.Count - 1);
           for (int i = 0; i < cellGroupsWithoutSums.Count; ++i)
           {
               int randomIndex = randomCellGroupIndex.Next();
               CellGroup cellGroup = cellGroupsWithoutSums.ElementAt(randomIndex);
               newCellGroups.Push(cellGroup);
           }
           cellGroupsWithoutSums = newCellGroups;
       }

       private void CleanOutAllCellValues()
       {
           foreach (CellGroup group in cellGroupsWithSums)
           {
               group.RevertToUnsolvedState();
           }
           foreach (CellGroup group in cellGroupsWithoutSums)
           {
               group.RevertToInitialState();  // took out RevertToUnsolvedState()
           }
           foreach (OpenCell cell in openCells)
           {
               cell.Initialize();
           }
       }

       Stack<CellGroup> cellGroupsWithSums;
       Stack<CellGroup> cellGroupsWithoutSums;
       Stack<OpenCell> openCells;
       public DebugLog debugLog;
   }
}
