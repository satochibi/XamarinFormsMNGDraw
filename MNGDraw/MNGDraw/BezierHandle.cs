using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MNGDraw
{
    public class BezierHandle
    {
        //絶対座標
        public SKPoint AnchorPoint { get; set; }

        //絶対座標
        private SKPoint mouseControlPoint;
        private SKPoint theOtherControlPoint;

        public SKPoint MouseControlPoint
        {
            get { return mouseControlPoint; }
            set
            {
                this.mouseControlPoint = value;
                this.theOtherControlPoint = ToOppositeVector(this.mouseControlPoint);
            }
        }

        public SKPoint TheOtherControlPoint
        {
            get { return theOtherControlPoint; }
            set
            {
                this.theOtherControlPoint = value;
                this.mouseControlPoint = ToOppositeVector(this.theOtherControlPoint);
            }
        }

        public float Magnitude
        {
            get { return (this.mouseControlPoint - this.AnchorPoint).Length; }
        }


        public BezierHandle(SKPoint aPoint)
        {
            this.AnchorPoint = aPoint;
            this.MouseControlPoint = aPoint;
        }

        public BezierHandle(SKPoint anAnchorPoint, SKPoint aMouseControlPoint)
        {
            this.AnchorPoint = anAnchorPoint;
            this.MouseControlPoint = aMouseControlPoint;
        }


        private SKPoint ToOppositeVector(SKPoint aPoint)
        {
            var temp = (this.mouseControlPoint - this.AnchorPoint);
            temp = new SKPoint(-temp.X, -temp.Y);
            return this.AnchorPoint + temp;
        }

    }
}
