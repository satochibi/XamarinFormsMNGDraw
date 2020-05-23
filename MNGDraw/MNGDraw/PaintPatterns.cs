using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MNGDraw
{
    public class PaintPatterns
    {
        private static SKBitmap Clear(SKBitmap bitmap)
        {
            bitmap.Erase(PaintColors.Transparent.ToSKColor());
            return bitmap;
        }

        public static SKBitmap NormalPattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(1, 1);
            tmp = Clear(tmp);
            tmp.SetPixel(0, 0, color);
            return tmp;
        }

        public static SKBitmap Rough3x3Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(3, 3);
            tmp = Clear(tmp);
            tmp.SetPixel(2, 2, color);
            return tmp;
        }

        public static SKBitmap Rough2x2Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(2, 2);
            tmp = Clear(tmp);
            tmp.SetPixel(1, 1, color);
            return tmp;
        }

        public static SKBitmap Beads4x4Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(4, 4);
            tmp = Clear(tmp);
            tmp.SetPixel(2, 0, color);
            tmp.SetPixel(1, 1, color);
            tmp.SetPixel(3, 1, color);
            tmp.SetPixel(0, 2, color);
            tmp.SetPixel(1, 3, color);
            tmp.SetPixel(3, 3, color);
            return tmp;
        }

        public static SKBitmap StripeXPattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(1, 2);
            tmp = Clear(tmp);
            tmp.SetPixel(0, 0, color);
            return tmp;
        }


        public static SKBitmap StripeYPattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(2, 1);
            tmp = Clear(tmp);
            tmp.SetPixel(0, 0, color);
            return tmp;
        }


        public static SKBitmap CheckeredPattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(2, 2);
            tmp = Clear(tmp);
            tmp.SetPixel(1, 0, color);
            tmp.SetPixel(0, 1, color);
            return tmp;
        }

        public static SKBitmap Dense2x2Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(2, 2);
            tmp = Clear(tmp);
            tmp.SetPixel(0, 0, color);
            tmp.SetPixel(1, 0, color);
            tmp.SetPixel(0, 1, color);
            return tmp;
        }

        public static SKBitmap Dense3x3Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(3, 3);
            tmp = Clear(tmp);
            tmp.SetPixel(0, 0, color);
            tmp.SetPixel(1, 0, color);
            tmp.SetPixel(2, 0, color);
            tmp.SetPixel(0, 1, color);
            tmp.SetPixel(1, 1, color);
            tmp.SetPixel(2, 1, color);
            tmp.SetPixel(0, 2, color);
            tmp.SetPixel(1, 2, color);
            return tmp;
        }

        public static SKBitmap LeftHatching8x8Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(8, 8);
            tmp = Clear(tmp);
            tmp.SetPixel(1, 0, color);
            tmp.SetPixel(2, 1, color);
            tmp.SetPixel(3, 2, color);
            tmp.SetPixel(4, 3, color);
            tmp.SetPixel(5, 4, color);
            tmp.SetPixel(6, 5, color);
            tmp.SetPixel(7, 6, color);
            tmp.SetPixel(0, 7, color);

            return tmp;
        }

        public static SKBitmap RightHatching8x8Pattern(SKColor color)
        {
            SKBitmap tmp = new SKBitmap(8, 8);
            tmp = Clear(tmp);
            tmp.SetPixel(7, 0, color);
            tmp.SetPixel(6, 1, color);
            tmp.SetPixel(5, 2, color);
            tmp.SetPixel(4, 3, color);
            tmp.SetPixel(3, 4, color);
            tmp.SetPixel(2, 5, color);
            tmp.SetPixel(1, 6, color);
            tmp.SetPixel(0, 7, color);
            return tmp;
        }

    }
}
