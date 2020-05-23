using System;
using System.Collections.Generic;
using System.Text;

namespace MNGDraw
{
    public struct PaintColors
    {
        public static Color Mask = new Color(0, 0, 0);
        public static Color Transparent = new Color(0, 0, 0, 0);
        public static Color ArtBoardBackground = new Color(230, 230, 230);
        public static Color OutOfRangeBackground = new Color(128, 128, 128);
        public static Color Black = new Color(20, 20, 20);
        public static Color White = new Color(255, 255, 255);
        public static Color Red = new Color(255, 23, 23);
        public static Color Yellow = new Color(255, 230, 0);
        public static Color Green = new Color(0, 130, 50);
        public static Color Blue = new Color(0, 60, 200);
        public static Color Lasso = new Color(127, 127, 255);
    }
}
