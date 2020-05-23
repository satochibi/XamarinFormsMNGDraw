using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

//https://paiza.io/projects/VrUkkxojXDY4WitgToiU1g
namespace MNGDraw
{
    public class Path : SKPath
    {
        public Color StrokeColor { get; set; }
        public float StrokeSize { get; set; }
        public float ArtBoardScaleFactor { get; set; } = 1;
        public SKStrokeJoin StrokeJoin { get; set; }
        public SKStrokeCap StrokeCap { get; set; }
        public SKBitmap Screentone { get; set; }
        public static double ScreentoneScale { get; set; }
        public bool IsScreentoneBlendMode { get; set; }



        public DateTime TimeStamp { get; set; }
        public string Author { get; set; }
        public bool IsVisibility { get; set; } = true;




        public Path()
        {
            this.Update();
        }

        public SKPaint GetStrokePaint()
        {
            SKPaint strokePaint = new SKPaint
            {
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                Color = this.StrokeColor.ToSKColor(),
                StrokeWidth = this.StrokeSize,
                StrokeJoin = this.StrokeJoin
            };

            return strokePaint;
        }

        public SKPaint GetStrokePaint(float artBoardScaleFactor)
        {
            SKPaint strokePaint = new SKPaint
            {
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                Color = this.StrokeColor.ToSKColor(),
                StrokeWidth = (int)(artBoardScaleFactor * this.StrokeSize / this.ArtBoardScaleFactor),
                StrokeJoin = this.StrokeJoin
            };

            return strokePaint;
        }


        public SKPaint GetFillPaint()
        {
            //新形式
            SKBitmap dstBitmap = new SKBitmap(
                new SKImageInfo(
                    this.Screentone.Info.Width * (int)ScreentoneScale,
                    this.Screentone.Info.Height * (int)ScreentoneScale));
            this.Screentone.ScalePixels(dstBitmap, SKFilterQuality.None);

            SKPaint fillPaint = new SKPaint
            {
                IsAntialias = false,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateBitmap(dstBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat)
            };
            return fillPaint;
        }

        public SKPaint GetFillBlendPaint(Color artBoardBackgroundColor)
        {
            Color color;
            if (this.IsScreentoneBlendMode)
            {
                color = PaintColors.Transparent;
            }
            else
            {
                color = artBoardBackgroundColor;
            }

            SKPaint fillBlendPaint = new SKPaint
            {
                IsAntialias = false,
                Style = SKPaintStyle.Fill,
                Color = color.ToSKColor()
            };

            return fillBlendPaint;
        }

        private void Update()
        {
            this.TimeStamp = DateTime.Now;

        }


        public new void Reset()
        {
            base.Reset();
            this.Update();
        }

        public void Complete(float artBoardScaleFactor)
        {
            this.ArtBoardScaleFactor = artBoardScaleFactor;
            this.Update();
        }



    }
}
