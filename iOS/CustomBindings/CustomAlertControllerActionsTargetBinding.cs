using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using MvvmCross.Platform.Platform;
using ProspectManagement.Core.Models;
using ProspectManagement.iOS.Utility;
using ProspectManagement.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.CustomBindings
{
    public class CustomAlertControllerActionsTargetBinding : MvxPropertyInfoTargetBinding<CustomAlertController>
    {
        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;
        public CustomAlertControllerActionsTargetBinding(object target, PropertyInfo targetPropertyInfo) : base(target, targetPropertyInfo)
        {

        }

        private void recreateAlertController(CustomAlertController view)
        {
            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Raleway-Light", 18f)
            };
            view.AlertController = UIAlertController.Create(view.AlertController.Title, "Choose a Value", UIAlertControllerStyle.ActionSheet);
            view.AlertController.SetValueForKey(new NSAttributedString(view.AlertController.Title, attributes), new NSString("attributedTitle"));

            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            actionCancel.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
            view.AlertController.AddAction(actionCancel);
        }

        protected override void SetValueImpl(object target, object value)
        {
            var view = Target as CustomAlertController;
            if (view == null) return;

            var codes = value as ObservableCollection<UserDefinedCode>;
            if (codes != null)
            {
                foreach (var code in codes)
                {
                    var action = UIAlertAction.Create(code.ToString(), UIAlertActionStyle.Default, a => { view.SelectedCode = code; });
                    action.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
                    view.AlertController.AddAction(action);
                }
            }
            else
            {
                var sourceCodes = value as ObservableCollection<TrafficSource>;
                if (sourceCodes != null)
                {
                    foreach (var code in sourceCodes)
                    {
                        var action = UIAlertAction.Create(code.ToString(), UIAlertActionStyle.Default, a => { view.SelectedCode = code; });
                        action.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
                        view.AlertController.AddAction(action);
                    }
                }
                else
                {
                    var detailCodes = value as List<TrafficSourceDetail>;
                    if (detailCodes != null)
                    {
                        if (view.AlertController.Actions.Count() > 1)
                        {
                            recreateAlertController(view);
                        }
                        foreach (var code in detailCodes)
                        {
                            var action = UIAlertAction.Create(code.ToString(), UIAlertActionStyle.Default, a => { view.SelectedCode = code; });
                            action.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
                            view.AlertController.AddAction(action);
                        }
                    }
                }
            }

            var actionCancel = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            actionCancel.SetValueForKey(ProspectManagementColors.DarkColor, new NSString("titleTextColor"));
            view.AlertController.AddAction(actionCancel);
        }
    }
}
