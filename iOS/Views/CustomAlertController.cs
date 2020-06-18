using System;
using Foundation;
using ProspectManagement.Core;
using ProspectManagement.iOS.Utility;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public class CustomAlertController
    {
        public event EventHandler SelectedCodeChanged;

        public void OnSelectedCodeChanged(EventArgs value)
        {
            SelectedCodeChanged?.Invoke(null, value);
        }

        private object _selectedCode;

        public object SelectedCode{
            get{
                return _selectedCode;
            }
            set{
                _selectedCode = value;
                OnSelectedCodeChanged(new ObjectEventArgs(_selectedCode));
            }
        }

        public UIAlertController AlertController
        {
            get;
            set;
        }

        public CustomAlertController(string alertTitle, string chooseMessage = "Choose a Value")
        {
            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Raleway-Light", 18f)
            };
            AlertController = UIAlertController.Create(alertTitle, chooseMessage, UIAlertControllerStyle.ActionSheet);
            AlertController.SetValueForKey(new NSAttributedString(alertTitle, attributes), new NSString("attributedTitle"));

            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            actionCancel.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
            AlertController.AddAction(actionCancel);
        }
    }
}
