using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MNGDraw
{
    public class Manga
    {
        private SKPoint aspectRatio;

        public string Title { get; set; }

        public string Description { get; set; }

        public string Quotation { get; set; }

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

        public List<Page> PageList { get; set; } = new List<Page>();

        public const int MaxPageNumber = 999;

        public const int MinPageNumber = 0;



        public Manga() : this(4, 3)
        {
        }

        public Manga(float aspectRatioX, float aspectRatioY)
        {
            this.AspectRatio = new SKPoint(aspectRatioX, aspectRatioY);
            this.PageList.Add(new Page());//最初は、新規ページを追加
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
    }
}
