using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums.types
{
    public enum UpdateResult { unchanged, changed, repaired };

    public class IllegalSumException2 : Exception
    {
        public IllegalSumException2(string message)
            : base(message)
        { }
    }
    public class UnsolvableGroupException2 : Exception
    {
        public UnsolvableGroupException2(string message)
            : base(message)
        { }
    }
}
