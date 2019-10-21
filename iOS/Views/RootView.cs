using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Converters;

namespace ProspectManagement.iOS.Views
{
	[MvxRootPresentation]
    public partial class RootView : MvxViewController<RootViewModel>
    {
        public RootView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<RootView, RootViewModel>();
            LoginActivityIndicator.StartAnimating();
            set.Bind(LoginActivityIndicator).For(v => v.Hidden).To(vm => vm.AttemptingLogin).WithConversion(new InverseValueConverter());
            set.Bind(AttemptingLoginLabel).For(b => b.Hidden).To(vm => vm.AttemptingLogin).WithConversion(new InverseValueConverter());
            set.Bind(LoginButton).For(b => b.Hidden).To(vm => vm.AttemptingLogin);
            set.Bind(LoginButton).To(vm => vm.LoginCommand);
            set.Apply();
        }
    }
}