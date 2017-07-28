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
    public partial class CobuyerView : MvxViewController<CobuyerViewModel>
    {
        public CobuyerView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<CobuyerView, CobuyerViewModel>();
            set.Bind(NameLabel).To(vm => vm.Prospect.Name);
            set.Bind(CobuyerDetailButton).To(vm => vm.ShowCobuyerDetail);
            set.Apply();

			foreach (UITabBarItem item in ProspectTabBar.Items)
			{
				if (item.Tag == 1)
				{
					item.SetTitleTextAttributes(new UITextAttributes()
					{
						Font = UIFont.FromName("Raleway-Bold", 10),
						TextColor = UIColor.Black
					}, UIControlState.Normal);

				}
				else
				{
					item.SetTitleTextAttributes(new UITextAttributes()
					{
						Font = UIFont.FromName("Raleway-Regular", 10),
						TextColor = UIColor.Black
					}, UIControlState.Normal);
				}
			}

            ProspectTabBar.SelectedItem = ProspectTabBar.Items[1];
            ProspectTabBar.ItemSelected += (sender, e) =>
            {
                if (e.Item.Tag == 0)
                    ViewModel.ShowDetailTab.Execute(null);
                else if (e.Item.Tag == 2)
                    ViewModel.ShowTrafficCardTab.Execute(null);
            };

        }
    }
}