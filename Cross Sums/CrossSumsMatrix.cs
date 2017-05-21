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
            while (!solvablePuzzleCreated)   // change "if" to "while" to retry indefinitely.
            {
                Initialize();
                try
                {
                    solvablePuzzleCreated = CreateThePuzzle();
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

       private bool CreateASymmetricOpenCellFoursome (Coordinates coord)
       {
           if ( (coord.X < 0) || (coord.Y < 0) || (coord.X+1 >= constants.dimensions) || (coord.Y+1 >= constants.dimensions) )
               return false;
           bool successful = true;
           if (!CreateTwoSymmetricOpenCells(new Coordinates(coord.X, coord.Y)))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(new Coordinates(coord.X + 1, coord.Y)))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(new Coordinates(coord.X, coord.Y + 1)))
               successful = false;
           if (!CreateTwoSymmetricOpenCells(new Coordinates(coord.X + 1, coord.Y + 1)))
               successful = false;
           return successful;
       }

       public bool CreateTwoSymmetricOpenCells(Coordinates coord)
       {
           if ((coord.X < 1) || (coord.Y < 1))
               return false;
           if ((coord.X >= constants.dimensions) || (coord.Y >= constants.dimensions))
               return false;
           ICell cell = cellMatrix[coord.X, coord.Y];
           if (cell == null)
           {
               debugLog.Write("creating an open cell at (" + coord.X + "," + coord.Y + ")\n");
               OpenCell newCell = new OpenCell(coord, this);
               cellMatrix[coord.X, coord.Y] = newCell;
               openCells.Push(newCell);
           }
           Coordinates symmCoord = new Coordinates
                           (constants.dimensions - coord.X, constants.dimensions - coord.Y);
           ICell symmetricCell = cellMatrix[symmCoord.X, symmCoord.Y];
           if (symmetricCell == null)
           {
               debugLog.Write("creating a symmetric open cell at (" + symmCoord.X + "," + symmCoord.Y + ")\n");
               OpenCell newCell = new OpenCell(symmCoord, this);
               cellMatrix[symmCoord.X, symmCoord.Y] = newCell;
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
           while(FindNextOpenColumn(ref coordinates))
           {
               CreateAColumnGroup(coordinates);
               if (coordinates.Y == constants.dimensions)
                   coordinates = new Coordinates(coordinates.X, coordinates.Y-1);
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
               coordinates = new Coordinates(coordinates.X, coordinates.Y + 1);
           }
           CellGroup newGroup = new ColumnCellGroup(columnSumCell, new List<OpenCell>(cellQueue), debugLog);
           CellGroup.columnList.Add(newGroup);
           cellGroupsWithoutSums.Push(newGroup);
       }

       private bool FindNextOpenColumn(ref Coordinates coords)
       {
           bool success = false;
           bool currentlyInAnOpenColumn = (CellIsOpen(coords)) ? true : false;    // Get past the open group we're in , so we can find the next one.

           int xCoordToTry = coords.X;
           int yCoordToTry = coords.Y;

           while (!success)
           {
               ++yCoordToTry;
               if (yCoordToTry >= constants.dimensions)
               {
                   ++xCoordToTry;
                   if (xCoordToTry >= constants.dimensions)  // reached the end of the matrix of cells.
                       break;
                   else
                   {
                       currentlyInAnOpenColumn = false;
                       yCoordToTry = 0;
                   }
               }
               if (currentlyInAnOpenColumn)
               {
                   if (!CellIsOpen(xCoordToTry, yCoordToTry))
                       currentlyInAnOpenColumn = false;
                   continue;
               }

               //Having reached this point, we know we are not in an open row, so if an open cell is found, return it.
               if (CellIsOpen(xCoordToTry, yCoordToTry))
               {
                   success = true;
                   break;
               }

           }
           if (success)
               coords = new Coordinates(xCoordToTry, yCoordToTry);
           return success;
       }

       private void CreateAllRowGroups()
       {
           Coordinates coordinates = new Coordinates(0, 0);
           while (FindNextOpenRow(ref coordinates))
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
               coordinates = new Coordinates(coordinates.X + 1, coordinates.Y); 
           }
           CellGroup newGroup = new RowCellGroup(rowSumCell, new List<OpenCell>(cellQueue), debugLog);
           CellGroup.rowList.Add(newGroup);
           cellGroupsWithoutSums.Push(newGroup);
       }

       private bool FindNextOpenRow(ref Coordinates coords)
       {
           bool success = false;
           bool currentlyInAnOpenRow = (CellIsOpen(coords)) ? true : false;    // Get past the open group we're in , so we can find the next one.

           int xCoordToTry = coords.X;
           int yCoordToTry = coords.Y;

           while (!success)
           {
               ++xCoordToTry;
               if (xCoordToTry >= constants.dimensions)
               {
                   ++yCoordToTry;
                   if (yCoordToTry >= constants.dimensions)  // reached the end of the matrix of cells.
                       break;
                   else
                   {
                       currentlyInAnOpenRow = false;
                       xCoordToTry = 0;
                   }
               }
               if (currentlyInAnOpenRow)
               {
                   if (!CellIsOpen(xCoordToTry, yCoordToTry))
                       currentlyInAnOpenRow = false;
                   continue;
               }

               //Having reached this point, we know we are not in an open row, so if an open cell is found, return it.
               if (CellIsOpen(xCoordToTry, yCoordToTry))
               {
                   success = true;
                   break;
               }

           }
           if (success)
               coords = new Coordinates(xCoordToTry, yCoordToTry);
           return success;
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
           Coordinates startingPoint = new Coordinates(3,3);
           Boolean edgeNotReached = true;

           // first pass to reach top/bottom edges.
           while (edgeNotReached)
           {
               List<int> selectionList = new List<int> { -1, 0, 1 };

               int xShift = Randomizer.RandomSelectFrom(selectionList);
               int yShift = 1;
               edgeNotReached = CreateASymmetricOpenCellFoursome(startingPoint);
               startingPoint = new Coordinates(startingPoint.X + xShift, startingPoint.Y + yShift);
           }

           // second pass to reach right/left edges.
           startingPoint = new Coordinates(3, 3);
           edgeNotReached = true;
           while (edgeNotReached)
           {
               List<int> selectionList = new List<int> { -1, 0, 1 };

               int xShift = 1;
               int yShift = Randomizer.RandomSelectFrom(selectionList);
               edgeNotReached = CreateASymmetricOpenCellFoursome(startingPoint);
               startingPoint = new Coordinates(startingPoint.X + xShift, startingPoint.Y + yShift);
           }

           CreateBlackCells();
           SetOpenCellTabOrder();
       }

       public bool CreateThePuzzle()
       {
           bool solvablePuzzleCreated = true;
           int maxNumberOfLoops = 50;
           CreateAllColumnGroups();
           CreateAllRowGroups();
           RandomlyOrderGroupsWithoutSums();
           //FindAllSquares();

           // 1. Select a random cellToTry, and pick a valid solution for it.  (Done here in CSM)
           // 2. Restrict possible solutions to all copyOfUnsolvedCellList in both parents.  (Done in the chosen parent group.)
           // 3. Remove completely solved groups from the puzzle.  (Done in the chosen parent group.)
           // 4. Are all the copyOfUnsolvedCellList solved?  No: go to 1,  Yes go to 5.  (Done here in CSM)
           // 5. Verify the solution?  Possibly not...

           int loopCount = 0;
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
               ++loopCount;
               debugLog.Write(CellGroup.PrintAllColumnSums());
               debugLog.Write(CellGroup.PrintAllRowSums());
               debugLog.Write("\n--------\nNumber of cell groups without sums left:"
                              + cellGroupsWithoutSums.Count + "    " + result + "\n--------\n");
               if (loopCount > maxNumberOfLoops)
               {
                   solvablePuzzleCreated = false;
                   break;
               }
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
