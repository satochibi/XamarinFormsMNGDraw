using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SkiaSharp;

namespace MNGDraw
{
    public class ArtBoard
    {
        private SKPoint aspectRatio;

        private SKPoint size;

        private float scaleFactor;

        public SKPoint AspectRatio
        {
            get { return this.aspectRatio; }
            set
            {
                var temp = value;
                var gcd = this.GCD((int)temp.X, (int)temp.Y);
                this.aspectRatio = new SKPoint(temp.X / gcd, temp.Y / gcd);
            }
        }

        public SKPoint AbsolutePosition { get; set; }

        public SKPoint Size
        {
            get { return this.size; }
        }

        public SKRect AbsoluteRect
        {
            get { return new SKRect(this.AbsolutePosition.X, this.AbsolutePosition.Y, this.AbsolutePosition.X + this.Size.X, this.AbsolutePosition.Y + this.Size.Y); }
        }

        public float ScaleFactor
        {
            get { return scaleFactor; }
        }


        public Path InProgressPath { get; set; } = new Path();

        public List<BezierHandle> BezierHandleList { get; set; } = new List<BezierHandle>();

        public Color BackgroundColor { get; set; } = PaintColors.ArtBoardBackground;

        public ArtBoard(SKPoint aspectRatio)
        {
            this.AspectRatio = aspectRatio;
        }



        public void SizeRecalculation(SKPoint surfaceSize)
        {

            SKRect artBoardRect = new SKRect();

            //if (artBoardWidth < surfaceWidth)
            if (aspectRatio.X * surfaceSize.Y < surfaceSize.X * aspectRatio.Y)
            {
                //画面が横長
                int artBoardHeight = (int)(surfaceSize.Y - (surfaceSize.Y % aspectRatio.Y));
                this.scaleFactor = (int)(artBoardHeight / aspectRatio.Y);
                int artBoardWidth = (int)(aspectRatio.X * this.scaleFactor);


                artBoardRect.Left = (int)((surfaceSize.X - artBoardWidth) / 2);
                artBoardRect.Top = 0;
                artBoardRect.Right = artBoardRect.Left + artBoardWidth;
                artBoardRect.Bottom = artBoardHeight;
            }
            else
            {
                //画面が縦長
                int artBoardWidth = (int)(surfaceSize.X - (surfaceSize.X % aspectRatio.X));
                this.scaleFactor = (int)(artBoardWidth / aspectRatio.X);
                int artBoardHeight = (int)(aspectRatio.Y * this.scaleFactor);

                artBoardRect.Left = 0;
                artBoardRect.Top = (int)((surfaceSize.Y - artBoardHeight) / 2);
                artBoardRect.Right = artBoardWidth;
                artBoardRect.Bottom = artBoardRect.Top + artBoardHeight;
            }


            this.AbsolutePosition = new SKPoint(artBoardRect.Left, artBoardRect.Top);
            this.size = new SKPoint(
                Math.Abs(artBoardRect.Right - artBoardRect.Left),
                Math.Abs(artBoardRect.Bottom - artBoardRect.Top));


        }

        /// <summary>
        /// 最大公約数を求める
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GCD(int x, int y)
        {
            if (y == 0)
            {
                return x;
            }

            return GCD(y, x % y);
        }

        public SKMatrix MatrixToAspectCoordinates()
        {
            SKMatrix A = SKMatrix.MakeTranslation(-this.AbsolutePosition.X, -this.AbsolutePosition.Y);
            SKMatrix B = SKMatrix.MakeScale(1.0f / this.ScaleFactor, 1.0f / this.ScaleFactor);
            SKMatrix.PostConcat(ref A, B);
            return A;
        }

        public SKMatrix InverseMatrixToAbsoluteCoordinates()
        {
            SKMatrix inverseMatrix;
            this.MatrixToAspectCoordinates().TryInvert(out inverseMatrix);
            return inverseMatrix;
        }


        public void GenerateBezierPath(PathPreviewAlgorithms pathPreview)
        {
            BezierHandle prevBezierHandle = null;
            bool firstMove = true;

            foreach (var aBezierHandle in this.BezierHandleList)
            {
                if (prevBezierHandle == null)
                {
                    prevBezierHandle = aBezierHandle;
                    continue;
                }

                if (firstMove)
                {
                    this.InProgressPath.MoveTo(prevBezierHandle.AnchorPoint);
                    firstMove = false;
                }

                this.InProgressPath.CubicTo(
                        prevBezierHandle.MouseControlPoint,
                        aBezierHandle.TheOtherControlPoint,
                        aBezierHandle.AnchorPoint);

                prevBezierHandle = aBezierHandle;
            }

            if (pathPreview == PathPreviewAlgorithms.LinearClose)
            {
                this.InProgressPath.Close();
            }
            else if (pathPreview == PathPreviewAlgorithms.Close)
            {
                this.InProgressPath.CubicTo(
                    this.BezierHandleList.Last().MouseControlPoint,
                    this.BezierHandleList.First().TheOtherControlPoint,
                    this.BezierHandleList.First().AnchorPoint);
                this.InProgressPath.Close();
            }


        }

    }
}
