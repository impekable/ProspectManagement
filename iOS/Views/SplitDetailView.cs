using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxDetailSplitViewPresentation(WrapInNavigationController = true)]
    public partial class SplitDetailView : MvxViewController<SplitDetailViewModel>
    {
        public SplitDetailView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var set = this.CreateBindingSet<SplitDetailView, SplitDetailViewModel>();
            set.Bind(NameLabel).To(vm => vm.Prospect.Name);
            set.Apply();
            			
            ProspectTabBar.ItemSelected += (sender, e) => {
                if (e.Item.Tag == 1)
                    ViewModel.ShowCobuyerTab.Execute(null);
				else if (e.Item.Tag == 2)
					ViewModel.ShowTrafficCardTab.Execute(null);
            };

			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("Raleway-Bold", 20)
			});
        }
    }
}