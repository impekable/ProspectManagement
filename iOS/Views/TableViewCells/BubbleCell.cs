using System;
using CoreGraphics;
using Foundation;
using ProspectManagement.Core.Models;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public abstract class BubbleCell : UITableViewCell
    {
        public UIImageView BubbleImageView { get; private set; }
        public UILabel MessageLabel { get; private set; }
        public UILabel DateLabel { get; private set; }
        public UIImage BubbleImage { get; set; }
        public UIImage BubbleHighlightedImage { get; set; }

        SmsActivity msg;

        public SmsActivity Message
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
                BubbleImageView.Image = BubbleImage;
                BubbleImageView.HighlightedImage = BubbleHighlightedImage;

                MessageLabel.Text = msg.MessageBody;

                MessageLabel.UserInteractionEnabled = true;
                BubbleImageView.UserInteractionEnabled = false;

                var t = TimeZoneInfo.Local;
                DateLabel.Text = msg.UpdatedDate.ToLocalTime().ToString() + " " + t.StandardName;
                DateLabel.Font = UIFont.FromName("Raleway-Italic", 14);
            }
        }

        public BubbleCell(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        public BubbleCell()
        {
            Initialize();
        }

        [Export("initWithStyle:reuseIdentifier:")]
        public BubbleCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(style, reuseIdentifier)
        {
            Initialize();
        }

        void Initialize()
        {
            BubbleImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            MessageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                PreferredMaxLayoutWidth = 220f
            };

            DateLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                PreferredMaxLayoutWidth = 220f
            };

            ContentView.AddSubviews(BubbleImageView, MessageLabel, DateLabel);
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            BubbleImageView.Highlighted = selected;
        }

        protected static UIImage CreateColoredImage(UIColor color, UIImage mask)
        {
            var rect = new CGRect(CGPoint.Empty, mask.Size);
            UIGraphics.BeginImageContextWithOptions(mask.Size, false, mask.CurrentScale);
            CGContext context = UIGraphics.GetCurrentContext();
            mask.DrawAsPatternInRect(rect);
            context.SetFillColor(color.CGColor);
            context.SetBlendMode(CGBlendMode.SourceAtop);
            context.FillRect(rect);
            UIImage result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return result;
        }

        protected static UIImage CreateBubbleWithBorder(UIImage bubbleImg, UIColor bubbleColor)
        {
            bubbleImg = CreateColoredImage(bubbleColor, bubbleImg);
            CGSize size = bubbleImg.Size;

            UIGraphics.BeginImageContextWithOptions(size, false, 0);
            var rect = new CGRect(CGPoint.Empty, size);
            bubbleImg.Draw(rect);

            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return result;
        }
    }
}
