using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ProspectManagement.iOS.Views
{
	public class HandwritingView: UIView
	{

        // clear the canvas
        public void Clear()
        {
            drawPath.Dispose();
            drawPath = new CGPath();
            fingerDraw = false;
            SetNeedsDisplay();

        }

        // pass in a reference to the controller, although I never use it
        //and could probably remove it
		public HandwritingView(RectangleF frame) : base(frame)
        {
            this.drawPath = new CGPath();
            this.BackgroundColor = UIColor.LightGray;
        }

        private CGPoint touchLocation;
        private CGPoint prevTouchLocation;
        private CGPath drawPath;
        private bool fingerDraw;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;
            this.fingerDraw = true;
            this.touchLocation = touch.LocationInView(this);
            this.prevTouchLocation = touch.PreviousLocationInView(this);
            this.SetNeedsDisplay();

        }

        public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;
            this.touchLocation = touch.LocationInView(this);
            this.prevTouchLocation = touch.PreviousLocationInView(this);
            this.SetNeedsDisplay();
        }


        public UIImage GetDrawingImage()
        {
            UIImage returnImg = null;

            UIGraphics.BeginImageContext(this.Bounds.Size);

            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                context.SetStrokeColor(UIColor.Black.CGColor);
                context.SetLineWidth(5f);
                context.SetLineJoin(CGLineJoin.Round);
                context.SetLineCap(CGLineCap.Round);
                context.AddPath(this.drawPath);
                context.DrawPath(CGPathDrawingMode.Stroke);
                returnImg = UIGraphics.GetImageFromCurrentImageContext();
            }

            UIGraphics.EndImageContext();

            return returnImg;
        }


        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (this.fingerDraw)
            {
                using (CGContext context = UIGraphics.GetCurrentContext())
                {
                    context.SetStrokeColor(UIColor.Black.CGColor);
                    context.SetLineWidth(5f);
                    context.SetLineJoin(CGLineJoin.Round);
                    context.SetLineCap(CGLineCap.Round);
                    this.drawPath.MoveToPoint(this.prevTouchLocation);
                    this.drawPath.AddLineToPoint(this.touchLocation);
                    context.AddPath(this.drawPath);
                    context.DrawPath(CGPathDrawingMode.Stroke);
                }
            }
        }
    }
}
