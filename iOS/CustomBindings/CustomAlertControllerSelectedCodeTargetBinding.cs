using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using ProspectManagement.iOS.Views;
using UIKit;

namespace ProspectManagement.iOS.CustomBindings
{
    public class CustomAlertControllerSelectedCodeTargetBinding : MvxPropertyInfoTargetBinding<CustomAlertController>
    {
        private bool _subscribed;
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;
        public CustomAlertControllerSelectedCodeTargetBinding(object target, PropertyInfo targetPropertyInfo) : base(target, targetPropertyInfo)
        {
        }

        public override void SubscribeToEvents()
        {
            var customAlertController = View;
            if (customAlertController == null)
            {
				return;
            }

            _subscribed = true;
            customAlertController.SelectedCodeChanged += HandleSelectedCodeChanged;
        }

        private void HandleSelectedCodeChanged(object sender, EventArgs e)
        {
            var customAlertController = View;
            if (customAlertController == null) return;

            FireValueChanged(customAlertController.SelectedCode);
        }

        protected override void SetValueImpl(object target, object value)
        {
            var view = Target as CustomAlertController;
            if (view == null) return;
        }

        // clean up
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (isDisposing)
            {
                var myView = View;
                if (myView != null && _subscribed)
                {
                    myView.SelectedCodeChanged -= HandleSelectedCodeChanged;
                    _subscribed = false;
                }
            }
        }
    }
}
