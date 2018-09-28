using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using System.Linq;
using ProspectManagement.iOS.Utility;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public partial class ActivityDetailView : MvxViewController<ActivityDetailViewModel>
    {

        protected ActivityDetailViewModel ActivityDetailViewModel => ViewModel as ActivityDetailViewModel;

        public ActivityDetailView(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<ActivityDetailView, ActivityDetailViewModel>();
            this.NavigationItem.Title = "Notes";
                            
            EmailWebView.LoadHtmlString(ActivityDetailViewModel.Notes, baseUrl: null);

            set.Apply();

            var closeButton = new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                ActivityDetailViewModel.CloseCommand.Execute(null);
            });

            closeButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = ProspectManagementColors.DarkColor
            }, UIControlState.Normal);

            this.NavigationItem.SetRightBarButtonItem(closeButton, true);
        }
    }
}
