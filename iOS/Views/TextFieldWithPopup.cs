using System;
using CoreGraphics;
using ProspectManagement.iOS.Utility;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public class TextFieldWithPopup
    {
        private static readonly int TextFieldMargin = 10;
        public static readonly int ArrowWidth = 14;

        private UIViewController _controller;
        private CustomAlertController _customAlertController;
        private UITextField _textField;
        private UIButton _button;

        public CustomAlertController CustomAlertController
        {
            get
            {
                return _customAlertController;
            }
            set
            {
                _customAlertController = value;
            }
        }

        private void CustomizeTextField()
        {
            _textField.TintColor = UIColor.Clear;

            var arrowImageView = new UIImageView(new CGRect(0, 0, ArrowWidth + TextFieldMargin, _textField.Frame.Height))
            {
                Image = UIImage.FromBundle("arrow-down.png"),
                ContentMode = UIViewContentMode.Left
            };
            _textField.RightView = arrowImageView;
            _textField.RightViewMode = UITextFieldViewMode.UnlessEditing;

            _textField.Layer.BorderColor = ProspectManagementColors.BorderColor.CGColor;
            _textField.Layer.BorderWidth = 1;

        }

        public TextFieldWithPopup(UITextField textField, UIViewController controller)
        {
            _controller = controller;
            _textField = textField;
            _button = new UIButton(new CGRect(0, 0, _textField.Bounds.Size.Width, _textField.Bounds.Size.Height));
            _button.AutoresizingMask = UIViewAutoresizing.All;
            _button.TouchUpInside += (sender, e) => {
                var popPresenter = _customAlertController.AlertController.PopoverPresentationController;
                if (popPresenter != null)
                {
                    popPresenter.SourceView = textField;
                    popPresenter.SourceRect = textField.Bounds;
                }
                _controller.PresentViewController(_customAlertController.AlertController, true, null);
            };
            _textField.AddSubview(_button);

            CustomizeTextField();

            _button.TranslatesAutoresizingMaskIntoConstraints = false;
            _button.TopAnchor.ConstraintEqualTo(_textField.TopAnchor, 0).Active = true;
            _button.BottomAnchor.ConstraintEqualTo(_textField.BottomAnchor, 0).Active = true;
            _button.LeadingAnchor.ConstraintEqualTo(_textField.LeadingAnchor, 0).Active = true;
            _button.TrailingAnchor.ConstraintEqualTo(_textField.TrailingAnchor, 0).Active = true;
        }
    }
}
