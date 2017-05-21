using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cross_Sums
{
    public partial class CrossSumsForm : Form
    {
        public CrossSumsForm()
        {
            InitializeComponent();
            debugLog = new DebugLog(this);
            cellMatrix = new CrossSumsMatrix(this);
            graphics = this.CreateGraphics();
        }

        public void Write(string strToWrite)
        {
            infoBox.AppendText(strToWrite);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawGrid(constants.dimensions);
            cellMatrix.DrawYourself();
            base.OnPaint(e);
        }
        private void DrawGrid(int dimensions)
        {
            int startPosition = constants.cellWidth;
            int length = constants.cellWidth * dimensions;
            int endPosition = startPosition + length;
            Rectangle theOuterBorder = new Rectangle(startPosition,
                                                 startPosition,
                                                 length,
                                                 length);
            graphics.FillRectangle(Brushes.Honeydew,
                                   theOuterBorder);
           
            //Pen drawingColor = new Pen(Brushes.Black, 2);
            //graphics.DrawRectangle(drawingColor, theOuterBorder);
         }
        public void DrawSumsCell(SumCell cell, int rowTotal, int columnTotal)
        {
            Point upperLeftPoint = UpperLeftLocation(cell.x, cell.y);
            Point lowerRightPoint = LowerRightLocation(cell.x, cell.y);
            Point upperRightPoint = UpperRightLocation(cell.x, cell.y);
            Point lowerLeftPoint = LowerLeftLocation(cell.x, cell.y);

            Point columnTotalLocation = new Point(upperLeftPoint.X + 5, upperLeftPoint.Y + 22);
            Point rowTotalLocation = new Point(upperLeftPoint.X + 19, upperLeftPoint.Y + 3);
            Font font = new Font("Biondi", 9);
            Point[] upperRightTriangle = {upperLeftPoint, upperRightPoint, lowerRightPoint};
            Point[] lowerLeftTriangle = {upperLeftPoint, lowerLeftPoint, lowerRightPoint};
            if (columnTotal != 0)
            {
                if (rowTotal == 0)
                    graphics.FillPolygon(Brushes.Black, upperRightTriangle);
                graphics.FillPolygon(Brushes.LightGray, lowerLeftTriangle);
                graphics.DrawPolygon(Pens.AntiqueWhite, upperRightTriangle);
                graphics.DrawPolygon(Pens.Black, lowerLeftTriangle);
                graphics.DrawString(columnTotal.ToString(), font, Brushes.Black, columnTotalLocation);
            }
            if (rowTotal != 0)
            {
                if (columnTotal == 0)
                    graphics.FillPolygon(Brushes.Black, lowerLeftTriangle);
                graphics.FillPolygon(Brushes.LightGray, upperRightTriangle);
                graphics.DrawPolygon(Pens.AntiqueWhite, lowerLeftTriangle);
                graphics.DrawPolygon(Pens.Black, upperRightTriangle);
                graphics.DrawString(rowTotal.ToString(), font, Brushes.Black, rowTotalLocation);
            }
            Pen linePen = new Pen(Brushes.Black, 2);
            graphics.DrawLine(linePen, upperLeftPoint, lowerRightPoint);
        }

        public void AddOpenCell(OpenCell cell)
        {
            Controls.Add(cell);
        }        
        private void ChangeElementCellValue(SumCell cell, int value)
        {
            Point valueLocation = new Point(UpperLeftLocation(cell.x, cell.y).X + 6,
                                            UpperLeftLocation(cell.x, cell.y).Y + 3);
            Font font = new Font("Constantia", 20);
            graphics.DrawString(value.ToString(), font, Brushes.Black, valueLocation);
        }
        public void DrawBlackBox(SumCell cell)
        {
            graphics.FillRectangle(Brushes.Black, GetRectangle(cell));
            graphics.DrawRectangle(Pens.LightGray, GetRectangle(cell));
        }

        /* Functions for drawing boxes */
        private Point UpperLeftLocation(int x, int y)
        {
            return new Point(constants.cellWidth * (x+1), constants.cellWidth * (y+1));
        }
        private Point UpperRightLocation(int x, int y)
        {
            return new Point(constants.cellWidth * (x+2), constants.cellWidth * (y+1));
        }
        public Point LowerRightLocation(int x, int y)
        {
            return new Point((constants.cellWidth * x) + 80, (constants.cellWidth * y) + 80);
        }
        public Point LowerLeftLocation(int x, int y)
        {
            return new Point(constants.cellWidth * (x+1), (constants.cellWidth * y) + 80);
        }
        private Rectangle GetRectangle(SumCell blackCell)
        {
            Size size = new Size(constants.cellWidth, constants.cellWidth);
            return new Rectangle(UpperLeftLocation(blackCell.x, blackCell.y), size);
        }

        private Graphics graphics;
        private CrossSumsMatrix cellMatrix;
        public static DebugLog debugLog;

        private void CrossSumsForm_Load(object sender, EventArgs e)
        {
        }
    }
}
