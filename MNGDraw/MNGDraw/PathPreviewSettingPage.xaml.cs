using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace MNGDraw
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PathPreviewSettingPage : ContentPage
    {
        ArtBoard anArtBoard = new ArtBoard(new SKPoint(1, 1));

        private float radius;

        private readonly float controlPointCoefficient = (float)((4 * (Math.Sqrt(2) - 1)) / 3);

        PathPreviewAlgorithms pathPreview;

        public PathPreviewSettingPage()
        {
            InitializeComponent();
        }
        private void PathPreviewSettingPage_Appearing(object sender, EventArgs e)
        {
            //読み込み
            pathPreviewPicker.SelectedIndex = SaveProperties.PathPreviewAlgorithms;
            Path.ScreentoneScale = SaveProperties.ScreentoneScale;

            pathPreviewCanvasView.InvalidateSurface();

        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            int surfaceWidth = info.Width;
            int surfaceHeight = info.Height;

            SKPoint surfaceSize = new SKPoint(surfaceWidth, surfaceHeight);


            //絵画開始
            //背景色を塗る
            canvas.Clear(PaintColors.OutOfRangeBackground.ToSKColor());

            //アートボードの絵画
            anArtBoard.SizeRecalculation(surfaceSize);

            using (SKPaint paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Fill;
                paint.Color = PaintColors.ArtBoardBackground.ToSKColor();
                canvas.DrawRect(anArtBoard.AbsoluteRect, paint);
            }


            radius = Math.Min(anArtBoard.Size.X, anArtBoard.Size.Y) / 2.0f * 0.7f;



            anArtBoard.InProgressPath.StrokeColor = PaintColors.Black;
            anArtBoard.InProgressPath.StrokeSize = 5;
            anArtBoard.InProgressPath.StrokeJoin = SKStrokeJoin.Round;
            anArtBoard.InProgressPath.StrokeCap = SKStrokeCap.Round;
            anArtBoard.InProgressPath.Screentone = PaintPatterns.CheckeredPattern(PaintColors.Blue.ToSKColor());

            anArtBoard.BezierHandleList.Clear();
            anArtBoard.BezierHandleList.Add(
                new BezierHandle(
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2, anArtBoard.Size.Y / 2 - radius),
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 + radius * controlPointCoefficient, anArtBoard.Size.Y / 2 - radius)));
            anArtBoard.BezierHandleList.Add(
                new BezierHandle(
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 + radius, anArtBoard.Size.Y / 2),
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 + radius, anArtBoard.Size.Y / 2 + radius * controlPointCoefficient)));
            anArtBoard.BezierHandleList.Add(
                new BezierHandle(
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2, anArtBoard.Size.Y / 2 + radius),
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 - radius * controlPointCoefficient, anArtBoard.Size.Y / 2 + radius)));
            anArtBoard.BezierHandleList.Add(
                new BezierHandle(
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 - radius, anArtBoard.Size.Y / 2),
                    anArtBoard.AbsolutePosition + new SKPoint(anArtBoard.Size.X / 2 - radius, anArtBoard.Size.Y / 2 - radius * controlPointCoefficient)));


            //絶対座標からアスペクト座標に変換
            foreach (var bezierHandle in anArtBoard.BezierHandleList)
            {
                bezierHandle.AnchorPoint = anArtBoard.MatrixToAspectCoordinates().MapPoint(bezierHandle.AnchorPoint);
                bezierHandle.MouseControlPoint = anArtBoard.MatrixToAspectCoordinates().MapPoint(bezierHandle.MouseControlPoint);
            }


            //暫定Path絵画
            if (anArtBoard.BezierHandleList.Count >= 2)
            {
                anArtBoard.InProgressPath.Reset();

                //bezierHandleList -> inProgressPath
                anArtBoard.GenerateBezierPath(pathPreview);
                anArtBoard.InProgressPath.Transform(this.anArtBoard.InverseMatrixToAbsoluteCoordinates());
                canvas.DrawPath(anArtBoard.InProgressPath, anArtBoard.InProgressPath.GetFillPaint());
                canvas.DrawPath(anArtBoard.InProgressPath, anArtBoard.InProgressPath.GetStrokePaint());
            }



            //コントロールポイントを絵画
            SKPaint aBezierHandlePaint = new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = PaintColors.Lasso.ToSKColor(),
                StrokeWidth = 2,
                StrokeJoin = SKStrokeJoin.Round,
                StrokeCap = SKStrokeCap.Round

            };

            SKPaint aBezierHandleAnchorPaint = new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = PaintColors.Lasso.ToSKColor()
            };



            using (SKPath path = new SKPath())
            {
                foreach (var aBezierHandle in anArtBoard.BezierHandleList)
                {
                    SKPoint anchorPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.AnchorPoint);
                    SKPoint mouseControlPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.MouseControlPoint);
                    SKPoint theOtherControlPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.TheOtherControlPoint);


                    path.MoveTo(anchorPoint);
                    path.LineTo(mouseControlPoint);

                    canvas.DrawCircle(anchorPoint, 4, aBezierHandleAnchorPaint);


                    path.MoveTo(anchorPoint);
                    path.LineTo(theOtherControlPoint);
                }

                canvas.DrawPath(path, aBezierHandlePaint);
            }


        }

        private void PathPreviewPickerChanged(object sender, EventArgs e)
        {
            switch (pathPreviewPicker.SelectedItem as string)
            {
                default:
                case "Open":
                    this.pathPreview = PathPreviewAlgorithms.Open;
                    break;
                case "LinearClose":
                    this.pathPreview = PathPreviewAlgorithms.LinearClose;
                    break;
                case "Close":
                    this.pathPreview = PathPreviewAlgorithms.Close;
                    break;
            }

            //保存
            SaveProperties.PathPreviewAlgorithms = pathPreviewPicker.SelectedIndex;

            pathPreviewCanvasView.InvalidateSurface();
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}