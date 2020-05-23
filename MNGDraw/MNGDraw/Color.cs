using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MNGDraw
{
    public class Color
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        public Color() : this(0, 0, 0, 0)
        {
        }

        public Color(byte red, byte green, byte blue, byte alpha = 0xff)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }


        public SKColor ToSKColor()
        {
            return new SKColor(this.Red, this.Green, this.Blue, this.Alpha);
        }
    }
}
