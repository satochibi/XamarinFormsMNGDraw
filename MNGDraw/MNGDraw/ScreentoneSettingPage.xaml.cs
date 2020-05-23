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
    public partial class ScreentoneSettingPage : ContentPage
    {
        public ScreentoneSettingPage()
        {
            InitializeComponent();
        }

        private void ScreentoneSettingPage_Appearing(object sender, EventArgs e)
        {
            //読み込み
            screentoneScaleStepper.Value = SaveProperties.ScreentoneScale;
        }



        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            int surfaceWidth = info.Width;
            int surfaceHeight = info.Height;

            int aBitmapWidth = surfaceWidth / 4;
            int aBitmapHeight = surfaceHeight / 2;

            //絵画開始
            //背景色を塗る
            canvas.Clear(PaintColors.ArtBoardBackground.ToSKColor());


            //SKBitmap srcBitmap = PaintPatterns.CheckeredPattern(PaintColors.Black);
            SKBitmap[] srcBitmapList = new SKBitmap[8];
            srcBitmapList[0] = PaintPatterns.Rough3x3Pattern(PaintColors.Black.ToSKColor());
            srcBitmapList[1] = PaintPatterns.Rough2x2Pattern(PaintColors.Black.ToSKColor());
            srcBitmapList[2] = PaintPatterns.Beads4x4Pattern(PaintColors.Black.ToSKColor());
            srcBitmapList[3] = PaintPatterns.StripeXPattern(PaintColors.Black.ToSKColor());
            srcBitmapList[4] = PaintPatterns.StripeYPattern(PaintColors.Black.ToSKColor());
            srcBitmapList[5] = PaintPatterns.CheckeredPattern(PaintColors.Black.ToSKColor());
            srcBitmapList[6] = PaintPatterns.Dense2x2Pattern(PaintColors.Black.ToSKColor());
            srcBitmapList[7] = PaintPatterns.Dense3x3Pattern(PaintColors.Black.ToSKColor());

            for (int index = 0; index < 8; index++)
            {
                SKBitmap dstBitmap = new SKBitmap(
                new SKImageInfo(
                    srcBitmapList[index].Info.Width * (int)screentoneScaleStepper.Value,
                    srcBitmapList[index].Info.Height * (int)screentoneScaleStepper.Value));
                srcBitmapList[index].ScalePixels(dstBitmap, SKFilterQuality.None);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = false;
                paint.Style = SKPaintStyle.Fill;
                paint.Shader = SKShader.CreateBitmap(dstBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);

                if (0 <= index && index <= 3)
                {
                    canvas.DrawRect(new SKRect(aBitmapWidth * index, 0, aBitmapWidth * (index + 1), aBitmapHeight), paint);
                }
                else
                {
                    canvas.DrawRect(new SKRect(aBitmapWidth * (index - 4), aBitmapHeight, aBitmapWidth * ((index - 4) + 1), aBitmapHeight * 2), paint);
                }
            }


            /*
            //旧形式
            SKImageInfo srcInfo = new SKImageInfo(
                srcBitmap.Width * (int)screentoneScaleStepper.Value,
                srcBitmap.Height * (int)screentoneScaleStepper.Value);
            SKBitmap dstBitmap = srcBitmap.Resize(srcInfo, SKBitmapResizeMethod.Box);
            

            
            //新形式
            SKBitmap dstBitmap = new SKBitmap(
                new SKImageInfo(
                    srcBitmap.Info.Width * (int)screentoneScaleStepper.Value,
                    srcBitmap.Info.Height * (int)screentoneScaleStepper.Value));
            srcBitmap.ScalePixels(dstBitmap,SKFilterQuality.None);
            

            
            paint.Shader = SKShader.CreateBitmap(
                dstBitmap,
                SKShaderTileMode.Repeat,
                SKShaderTileMode.Repeat);
                

            canvas.DrawRect(rect, paint);
            */

        }



        private void ScreentoneScaleStepperChanged(object sender, EventArgs e)
        {
            //保存
            SaveProperties.ScreentoneScale = screentoneScaleStepper.Value;
            screentonePreviewCanvasView.InvalidateSurface();
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}