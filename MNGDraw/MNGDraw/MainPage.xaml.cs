using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace MNGDraw
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Manga aManga = new Manga();

        private ArtBoard anArtBoard;

        private int currentPageNumber = Manga.MinPageNumber;
        private int currentLayerNumber = 0;


        public MainPage()
        {
            anArtBoard = new ArtBoard(aManga.AspectRatio);
            InitializeComponent();
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            //Application.Current.Properties.Clear();

            //PenTipを初期化
            this.PenTipChanged();
            //ページナンバーを初期化
            this.CurrentPageNumberChenged();
            canvasView.InvalidateSurface();
        }



        //https://github.com/mono/SkiaSharp/releases/tag/v1.58.1
        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            //絶対座標
            SKPoint absoluteTouchLocation = new SKPoint((int)Math.Floor(e.Location.X), (int)Math.Floor(e.Location.Y));
            //アスペクト座標に変換
            SKPoint aspectTouchLocation = anArtBoard.MatrixToAspectCoordinates().MapPoint(absoluteTouchLocation);

            // the ID of the finger that toucged the screen
            var pointerId = e.Id;





            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // the user added a finger
                    inProgressPathUndoButton.IsEnabled = false;
                    openPathButton.IsEnabled = false;
                    closePathButton.IsEnabled = false;

                    anArtBoard.BezierHandleList.Add(new BezierHandle(aspectTouchLocation));
                    canvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Moved:
                    // the user moved a finger
                    if (e.InContact)
                    {
                        if (anArtBoard.BezierHandleList.Count == 0)
                        {
                            break;
                        }

                        inProgressPathUndoButton.IsEnabled = false;
                        openPathButton.IsEnabled = false;
                        closePathButton.IsEnabled = false;

                        anArtBoard.BezierHandleList.Last().MouseControlPoint = aspectTouchLocation;
                        if (anArtBoard.BezierHandleList.Last().Magnitude <= SaveProperties.BezierHandleThreshold)
                        {
                            anArtBoard.BezierHandleList.Last().MouseControlPoint = anArtBoard.BezierHandleList.Last().AnchorPoint;
                        }
                    }

                    canvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Released:
                    // the user removed a finger
                    if (anArtBoard.BezierHandleList.Count == 0)
                    {
                        break;
                    }

                    inProgressPathUndoButton.IsEnabled = true;
                    openPathButton.IsEnabled = true;
                    closePathButton.IsEnabled = true;

                    anArtBoard.BezierHandleList.Last().MouseControlPoint = aspectTouchLocation;
                    if (anArtBoard.BezierHandleList.Last().Magnitude <= SaveProperties.BezierHandleThreshold)
                    {
                        anArtBoard.BezierHandleList.Last().MouseControlPoint = anArtBoard.BezierHandleList.Last().AnchorPoint;
                    }

                    canvasView.InvalidateSurface();
                    break;
                case SKTouchAction.Cancelled:
                    // the user removed a finger
                    inProgressPathUndoButton.IsEnabled = true;
                    openPathButton.IsEnabled = true;
                    closePathButton.IsEnabled = true;

                    anArtBoard.BezierHandleList.RemoveAt(anArtBoard.BezierHandleList.Count - 1);
                    canvasView.InvalidateSurface();
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
                paint.Color = anArtBoard.BackgroundColor.ToSKColor();
                canvas.DrawRect(anArtBoard.AbsoluteRect, paint);
            }




            //completedPathList絵画
            //foreach (var aLayer in aManga.PageList[this.currentPageNumber].LayerList)の逆順: (2→1→0)
            for (int index = aManga.PageList[this.currentPageNumber].LayerList.Count - 1; 0 <= index; index--)
            {

                var aLayer = aManga.PageList[this.currentPageNumber].LayerList[index];

                foreach (var aCompletedPath in aLayer.CompletedPathList)
                {
                    if (!aCompletedPath.IsVisibility)
                    {
                        continue;
                    }
                    SKPath aPath = new SKPath(aCompletedPath);//ディープコピー(aCompletedPathが拡大され続けるのを防ぐ)
                    aPath.Transform(this.anArtBoard.InverseMatrixToAbsoluteCoordinates());

                    canvas.DrawPath(aPath, aCompletedPath.GetFillBlendPaint(anArtBoard.BackgroundColor));
                    canvas.DrawPath(aPath, aCompletedPath.GetFillPaint());
                    canvas.DrawPath(aPath, aCompletedPath.GetStrokePaint(this.anArtBoard.ScaleFactor));
                }
            }



            //暫定Path絵画
            if (anArtBoard.BezierHandleList.Count >= 2)
            {
                anArtBoard.InProgressPath.Reset();

                //bezierHandleList -> inProgressPath
                anArtBoard.GenerateBezierPath((PathPreviewAlgorithms)SaveProperties.PathPreviewAlgorithms);
                anArtBoard.InProgressPath.Transform(this.anArtBoard.InverseMatrixToAbsoluteCoordinates());

                canvas.DrawPath(anArtBoard.InProgressPath, anArtBoard.InProgressPath.GetFillBlendPaint(anArtBoard.BackgroundColor));
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




        /*
         * ボタン開始
         *
         */



        private void InProgressPathUndoButtonClicked(object sender, EventArgs e)
        {
            if (anArtBoard.BezierHandleList.Count == 0)
            {
                return;
            }

            anArtBoard.BezierHandleList.RemoveAt(anArtBoard.BezierHandleList.Count - 1);
            canvasView.InvalidateSurface();

        }

        private void OpenPathButtonClicked(object sender, EventArgs e)
        {
            if (anArtBoard.BezierHandleList.Count <= 1)
            {
                DisplayAlert("アンカーポイントが1個以下です", "パスを絵画するためには2個以上必要です", "OK");
                return;
            }

            anArtBoard.InProgressPath.Reset();
            anArtBoard.GenerateBezierPath(PathPreviewAlgorithms.Open);
            anArtBoard.InProgressPath.Complete(anArtBoard.ScaleFactor);
            aManga.PageList[this.currentPageNumber].LayerList[this.currentLayerNumber].CompletedPathList.Add(anArtBoard.InProgressPath);
            anArtBoard.BezierHandleList.Clear();
            anArtBoard.InProgressPath = new Path();
            PenTipChanged();
            canvasView.InvalidateSurface();
        }

        private void ClosePathButtonClicked(object sender, EventArgs e)
        {
            if (anArtBoard.BezierHandleList.Count <= 1)
            {
                DisplayAlert("アンカーポイントが1個以下です", "パスを絵画するためには2個以上必要です", "OK");
                return;
            }
            anArtBoard.InProgressPath.Reset();
            anArtBoard.GenerateBezierPath(PathPreviewAlgorithms.Close);
            anArtBoard.InProgressPath.Complete(anArtBoard.ScaleFactor);
            aManga.PageList[this.currentPageNumber].LayerList[this.currentLayerNumber].CompletedPathList.Add(anArtBoard.InProgressPath);
            anArtBoard.BezierHandleList.Clear();
            anArtBoard.InProgressPath = new Path();
            PenTipChanged();
            canvasView.InvalidateSurface();
        }

        private void UndoButtonClicked(object sender, EventArgs e)
        {
            if (aManga.PageList[this.currentPageNumber].LayerList[this.currentLayerNumber].CompletedPathList.Count == 0)
            {
                return;
            }

            aManga.PageList[this.currentPageNumber].LayerList[this.currentLayerNumber].CompletedPathList.RemoveAt(
                aManga.PageList[this.currentPageNumber].LayerList[this.currentLayerNumber].CompletedPathList.Count - 1
                );
            canvasView.InvalidateSurface();
        }


        private void MenuButtonClicked(object sender, EventArgs e)
        {
            
            var menuPage = new MenuPage();
            Navigation.PushAsync(menuPage);
            menuPage.SetMangaOfMainPage(aManga);
            
        }

        /*
        private void ClearAllButtonClicked(object sender, EventArgs e)
        {
            if (anArtBoard.CompletedPathList.Count == 0)
            {
                return;
            }

            anArtBoard.CompletedPathList.Clear();
            canvasView.InvalidateSurface();
        }
        */

        private void ScreentoneBlendModeSwitchToggled(object sender, ToggledEventArgs e)
        {
            PenTipChanged();
            canvasView.InvalidateSurface();
        }

        private void StrokeColorPickerChanged(object sender, EventArgs e)
        {
            PenTipChanged();
            canvasView.InvalidateSurface();
        }

        private void FillColorPickerChanged(object sender, EventArgs e)
        {
            PenTipChanged();
            canvasView.InvalidateSurface();
        }

        private void FillPatternPickerChanged(object sender, EventArgs e)
        {
            PenTipChanged();
            canvasView.InvalidateSurface();
        }



        private void StrokeSizeStepperChanged(object sender, EventArgs e)
        {
            PenTipChanged();
            canvasView.InvalidateSurface();
        }



        private void PenTipChanged()
        {
            //ペン先のstrokeカラー設定
            Color strokeColor = PaintColors.Transparent;
            switch (strokeColorPicker.SelectedItem as string)
            {
                default:
                case "None":
                    strokeColor = PaintColors.Transparent;
                    break;
                case "Black":
                    strokeColor = PaintColors.Black;
                    break;
                case "White":
                    strokeColor = PaintColors.White;
                    break;
                case "Red":
                    strokeColor = PaintColors.Red;
                    break;
                case "Yellow":
                    strokeColor = PaintColors.Yellow;
                    break;
                case "Green":
                    strokeColor = PaintColors.Green;
                    break;
                case "Blue":
                    strokeColor = PaintColors.Blue;
                    break;
            }



            anArtBoard.InProgressPath.StrokeColor = strokeColor;
            anArtBoard.InProgressPath.StrokeSize = (float)strokeSizeStepper.Value;
            anArtBoard.InProgressPath.StrokeJoin = SKStrokeJoin.Round;
            anArtBoard.InProgressPath.StrokeCap = SKStrokeCap.Round;
            anArtBoard.InProgressPath.IsScreentoneBlendMode = screentoneBlendModeSwitch.IsToggled;



            //ペン先のfillカラー設定
            Color fillColor = PaintColors.Transparent;
            switch (fillColorPicker.SelectedItem as string)
            {
                default:
                case "None":
                    fillColor = PaintColors.Transparent;
                    break;
                case "Black":
                    fillColor = PaintColors.Black;
                    break;
                case "White":
                    fillColor = PaintColors.White;
                    break;
                case "Red":
                    fillColor = PaintColors.Red;
                    break;
                case "Yellow":
                    fillColor = PaintColors.Yellow;
                    break;
                case "Green":
                    fillColor = PaintColors.Green;
                    break;
                case "Blue":
                    fillColor = PaintColors.Blue;
                    break;
            }

            SKBitmap bitmap = new SKBitmap();

            //ペン先のパターン設定
            switch (fillPatternPicker.SelectedItem as string)
            {
                default:
                case "Normal":
                    bitmap = PaintPatterns.NormalPattern(fillColor.ToSKColor());
                    break;
                case "Rough3x3":
                    bitmap = PaintPatterns.Rough3x3Pattern(fillColor.ToSKColor());
                    break;
                case "Rough2x2":
                    bitmap = PaintPatterns.Rough2x2Pattern(fillColor.ToSKColor());
                    break;
                case "Beads4x4":
                    bitmap = PaintPatterns.Beads4x4Pattern(fillColor.ToSKColor());
                    break;
                case "StripeX":
                    bitmap = PaintPatterns.StripeXPattern(fillColor.ToSKColor());
                    break;
                case "StripeY":
                    bitmap = PaintPatterns.StripeYPattern(fillColor.ToSKColor());
                    break;
                case "Checkered":
                    bitmap = PaintPatterns.CheckeredPattern(fillColor.ToSKColor());
                    break;
                case "Dense2x2":
                    bitmap = PaintPatterns.Dense2x2Pattern(fillColor.ToSKColor());
                    break;
                case "Dense3x3":
                    bitmap = PaintPatterns.Dense3x3Pattern(fillColor.ToSKColor());
                    break;
                case "LeftHatching":
                    bitmap = PaintPatterns.LeftHatching8x8Pattern(fillColor.ToSKColor());
                    break;
                case "RightHatching":
                    bitmap = PaintPatterns.RightHatching8x8Pattern(fillColor.ToSKColor());
                    break;
            }


            anArtBoard.InProgressPath.Screentone = bitmap;
            Path.ScreentoneScale = SaveProperties.ScreentoneScale;
        }

        private void PagePreviousButtonClicked(object sender, EventArgs e)
        {
            if (currentPageNumber <= Manga.MinPageNumber)
            {
                return;
            }
            this.currentPageNumber--;
            this.CurrentPageNumberChenged();
            canvasView.InvalidateSurface();

        }

        private void PagePlayButtonClicked(object sender, EventArgs e)
        {
            //
            this.CurrentPageNumberChenged();

        }

        private void PageNextButtonClicked(object sender, EventArgs e)
        {
            if (currentPageNumber >= Manga.MaxPageNumber)
            {
                return;
            }
            this.currentPageNumber++;

            //範囲外になったら、新しくページを作る
            if (this.currentPageNumber > aManga.PageList.Count - 1)
            {
                aManga.PageList.Add(new Page());
            }

            this.CurrentPageNumberChenged();
            canvasView.InvalidateSurface();

        }

        private void CurrentPageNumberChenged()
        {
            //0ページなら前ページ遷移ボタンを無効化
            if (this.currentPageNumber <= Manga.MinPageNumber)
            {
                pagePreviousButton.IsEnabled = false;
            }
            else
            {
                pagePreviousButton.IsEnabled = true;

            }

            //999ページなら次ページ遷移ボタンを無効化
            if (this.currentPageNumber >= Manga.MaxPageNumber)
            {
                pageNextButton.IsEnabled = false;
            }
            else
            {
                pageNextButton.IsEnabled = true;
            }


            currentPageLabel.Text = "Page: " + (currentPageNumber + 1) + " / " + aManga.PageList.Count;
        }

        private void LayerPickerChanged(object sender, EventArgs e)
        {
            switch (layerPicker.SelectedItem as string)
            {
                default:
                case "A":
                    currentLayerNumber = 0;
                    break;
                case "B":
                    currentLayerNumber = 1;
                    break;
                case "C":
                    currentLayerNumber = 2;
                    break;
            }
            canvasView.InvalidateSurface();

        }

    }
}
