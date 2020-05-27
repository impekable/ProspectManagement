using System;

using Foundation;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public partial class OutgoingCell : BubbleCell
    {
        static readonly UIImage normalBubbleImage;
        static readonly UIImage highlightedBubbleImage;

        public static readonly NSString Key = new NSString("OutgoingCell");
        public static readonly UINib Nib;

        static OutgoingCell()
        {
            UIImage mask = UIImage.FromBundle("BubbleOutgoing");

            var cap = new UIEdgeInsets
            {
                Top = 17f,
                Left = 21f,
                Bottom = 17f,
                Right = 26f
            };

            var normalColor = UIColor.FromRGB(43, 119, 250);
            var highlightedColor = UIColor.FromRGB(32, 96, 200);

            normalBubbleImage = CreateColoredImage(normalColor, mask).CreateResizableImage(cap);
            highlightedBubbleImage = CreateColoredImage(highlightedColor, mask).CreateResizableImage(cap);
        }

        public OutgoingCell(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        public OutgoingCell()
        {
            Initialize();
        }

        [Export("initWithStyle:reuseIdentifier:")]
        public OutgoingCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(style, reuseIdentifier)
        {
            Initialize();
        }

        void Initialize()
        {
            BubbleHighlightedImage = highlightedBubbleImage;
            BubbleImage = normalBubbleImage;

            var views = new NSMutableDictionary();
            views.Add(new NSString("bubble"), BubbleImageView);
            views.Add(new NSString("date"), DateLabel);

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:[bubble]|", 0, "bubble", BubbleImageView));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-2-[bubble]-[date]-2-|", 0, null, views));

            BubbleImageView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:[bubble(>=48)]",0,"bubble", BubbleImageView));

            var vSpaceTop = NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.Top, 1, 10);
            ContentView.AddConstraint(vSpaceTop);

            var vSpaceBottom = NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.Bottom, 1, -10);
            ContentView.AddConstraint(vSpaceBottom);

            var msgTrailing = NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, BubbleImageView, NSLayoutAttribute.Trailing, 1, -16);
            ContentView.AddConstraint(msgTrailing);

            var msgCenter = NSLayoutConstraint.Create(MessageLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.CenterX, 1, -3);
            ContentView.AddConstraint(msgCenter);

            var vSpaceTopDate = NSLayoutConstraint.Create(DateLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.Bottom, 1, 3);
            ContentView.AddConstraint(vSpaceTopDate);

            var dateLeading = NSLayoutConstraint.Create(DateLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, BubbleImageView, NSLayoutAttribute.Leading, 1, 16);
            ContentView.AddConstraint(dateLeading);

            var dateCenter = NSLayoutConstraint.Create(DateLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.CenterX, 1, 3);
            ContentView.AddConstraint(dateCenter);

            MessageLabel.TextColor = UIColor.White;
        }
    }
}
