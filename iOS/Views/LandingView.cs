using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.ViewModels;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxRootPresentation(AnimationOptions = UIViewAnimationOptions.CurveEaseIn, AnimationDuration = 0.3f )]
    public partial class LandingView : MvxViewController<LandingViewModel>
    {
        private NSObject _notificationHandle;

        public override void ViewWillAppear(bool animated)
        {
            if (_notificationHandle == null)
            {
                _notificationHandle = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            }
        }

        private void HandleAppWillEnterForeground(NSNotification notification)
        {
            System.Diagnostics.Debug.WriteLine("Being notified of ViewWillAppear " + this.Handle);
            var timeSinceLoad = DateTime.Now - ViewModel.LoadTime;
            if (timeSinceLoad.TotalMinutes > 5)
            {
                ViewModel.GoHomeCommand.Execute(null);               
            }
        }

        public LandingView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var set = this.CreateBindingSet<LandingView, LandingViewModel>();
            set.Bind(NameLabel).To(vm => vm.User.AddressBook.Name);
            set.Bind(UnreadSMSLabel).To(vm => vm.User.UnreadSMSCount).WithConversion(new UnreadSMSMessageCountConverter());
            set.Bind(ViewDailyToDoButton).To(vm => vm.ViewDailyToDoCommand);
            set.Bind(ViewProspectsButton).To(vm => vm.ViewProspectsCommand);
            set.Bind(ViewSMSInboxButton).To(vm => vm.ViewSMSInboxCommand);
            set.Bind(LogoutButton).To(vm => vm.LogoutCommand);
            set.Apply();
        }
    }
}