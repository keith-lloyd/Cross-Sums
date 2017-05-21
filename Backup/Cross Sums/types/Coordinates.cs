using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums.types
{
    public class Coordinates
    {
        public Coordinates(int xValue, int yValue)
        {
            X = xValue;
            Y = yValue;
        }
        public static bool operator <(Coordinates coord1, Coordinates coord2)
        {
            if ((coord1.X < coord2.X) && (coord1.Y < coord2.Y))
                return true;
            return false;
        }
        public static bool operator >(Coordinates coord1, Coordinates coord2)
        {
            if ((coord1.X > coord2.X) && (coord1.Y > coord2.Y))
                return true;
            return false;
        }
        public int X;
        public int Y;
    }
}
