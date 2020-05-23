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
    public partial class BezierHandleSettingPage : ContentPage
    {
        
        private BezierHandle aBezierHandle = null;

        private SKPoint aspectRatio;

        private ArtBoard anArtBoard;

        public BezierHandleSettingPage()
        {
            InitializeComponent();
        }

        private void BezierHandleSettingPage_Appearing(object sender, EventArgs e)
        {
            //読み込み
            bezierHandleThresholdSlider.Value = SaveProperties.BezierHandleThreshold;
        }

        public void SetMangaOfMainPage(Manga aManga)
        {
            this.aspectRatio = aManga.AspectRatio;
            this.bezierHandleThresholdSlider.BindingContext = this;
            anArtBoard = new ArtBoard(this.aspectRatio);
        }

        public float AspectRatioMax
        {
            get { return Math.Max(this.aspectRatio.X, this.aspectRatio.Y) / 4.0f; }
        }


        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            SKPoint absoluteTouchLocation = new SKPoint((int)Math.Floor(e.Location.X), (int)Math.Floor(e.Location.Y));

            SKPoint aspectTouchLocation = anArtBoard.MatrixToAspectCoordinates().MapPoint(absoluteTouchLocation);

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // the user added a finger
                    aBezierHandle = new BezierHandle(aspectTouchLocation);
                    bezierHandlePreviewCanvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Moved:
                    // the user moved a finger
                    if (e.InContact)
                    {
                        aBezierHandle.MouseControlPoint = aspectTouchLocation;
                        if (aBezierHandle.Magnitude <= bezierHandleThresholdSlider.Value)
                        {
                            aBezierHandle.MouseControlPoint = aBezierHandle.AnchorPoint;
                        }
                    }
                    bezierHandlePreviewCanvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Released:
                    // the user removed a finger
                    aBezierHandle.MouseControlPoint = aspectTouchLocation;
                    if (aBezierHandle.Magnitude <= bezierHandleThresholdSlider.Value)
                    {
                        aBezierHandle.MouseControlPoint = aBezierHandle.AnchorPoint;
                    }
                    bezierHandlePreviewCanvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Cancelled:
                    // the user removed a finger
                    aBezierHandle = null;
                    bezierHandlePreviewCanvasView.InvalidateSurface();
                    break;
            }

            // the location (in pixels) of the finger on the screen
            //var pos = e.Location;

            // set Handled to true if we handled the event
            // if we don't, then parent views may also respond
            e.Handled = true;
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

            if (aBezierHandle == null)
            {
                return;
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
                SKPoint anchorPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.AnchorPoint);
                SKPoint mouseControlPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.MouseControlPoint);
                SKPoint theOtherControlPoint = anArtBoard.InverseMatrixToAbsoluteCoordinates().MapPoint(aBezierHandle.TheOtherControlPoint);

                path.MoveTo(anchorPoint);
                path.LineTo(mouseControlPoint);

                canvas.DrawCircle(anchorPoint, 4, aBezierHandleAnchorPaint);

                path.MoveTo(anchorPoint);
                path.LineTo(theOtherControlPoint);

                canvas.DrawPath(path, aBezierHandlePaint);
            }

        }

        private void BezierHandleThresholdSliderChanged(object sender, ValueChangedEventArgs e)
        {
            //保存
            SaveProperties.BezierHandleThreshold = bezierHandleThresholdSlider.Value;

            if (aBezierHandle == null)
            {
                return;
            }

            Application.Current.SavePropertiesAsync();
            if (aBezierHandle.Magnitude <= bezierHandleThresholdSlider.Value)
            {
                aBezierHandle.MouseControlPoint = aBezierHandle.AnchorPoint;
                bezierHandlePreviewCanvasView.InvalidateSurface();
            }
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}