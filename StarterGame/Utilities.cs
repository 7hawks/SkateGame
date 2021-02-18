using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class Utilities
    {
        public static int Scale(int input, double scale)
        {
            return (int)Math.Round(input * scale);
        }

    }
}
