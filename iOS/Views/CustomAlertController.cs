using System;
using Foundation;
using ProspectManagement.iOS.Utility;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public class CustomAlertController
    {
        public event EventHandler SelectedCodeChanged;

        public void OnSelectedCodeChanged()
        {
            SelectedCodeChanged?.Invoke(null, EventArgs.Empty);
        }

        private object _selectedCode;

        public object SelectedCode{
            get{
                return _selectedCode;
            }
            set{
                _selectedCode = value;
                OnSelectedCodeChanged();
            }
        }

        public UIAlertController AlertController
        {
            get;
            set;
        }

        public CustomAlertController(string alertTitle)
        {
            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Raleway-Light", 18f)
            };
            AlertController = UIAlertController.Create(alertTitle, "Choose a Value", UIAlertControllerStyle.ActionSheet);
            AlertController.SetValueForKey(new NSAttributedString(alertTitle, attributes), new NSString("attributedTitle"));

            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            actionCancel.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
            AlertController.AddAction(actionCancel);
        }
    }
}
