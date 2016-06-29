using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KRSP.Utils
{
    public class ColorUtils
    {
        public static int ColorToWin32Int(Color color)
        {
            return ColorTranslator.ToWin32(color);
        }
    }
}
