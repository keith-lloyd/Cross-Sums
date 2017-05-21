using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cross_Sums
{
    public interface ICell
    {
        void DrawYourself(CrossSumsForm form);
        bool AssumeFocus();
    }
}
