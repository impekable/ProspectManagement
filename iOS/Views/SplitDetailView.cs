using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Converters;

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
            set.Bind(AddressLine1Label).To(vm => vm.Prospect.StreetAddress.AddressLine1);
            set.Bind(CityStateZipLabel).To(vm => vm.Prospect.StreetAddress).WithConversion(new CityStateZipConverter()); ;
            set.Bind(EmailLabel).To(vm => vm.Prospect.Email.EmailAddress);
            set.Bind(WorkPhoneLabel).To(vm => vm.Prospect.WorkPhoneNumber.Phone);
            set.Bind(WorkLabel).To(vm => vm.WorkPhoneLabel);
            set.Bind(HomePhoneLabel).To(vm => vm.Prospect.HomePhoneNumber.Phone);
            set.Bind(HomeLabel).To(vm => vm.HomePhoneLabel);
            set.Bind(MobilePhoneLabel).To(vm => vm.Prospect.MobilePhoneNumber.Phone);
            set.Bind(MobileLabel).To(vm => vm.MobilePhoneLabel);
            set.Bind(ConsentLabel).To(vm => vm.Prospect.FollowUpSettings).WithConversion(new FollowUpSettingsValueConverter());
            set.Bind(ContactPreferenceLabel).To(vm => vm.Prospect.FollowUpSettings.PreferredContactMethod);
            set.Bind(AssignButton).For(c => c.Hidden).To(vm => vm.ProspectAssigned);
            set.Bind(AssignButton).To(vm => vm.AssignCommand);
            set.Apply();
            			
            ProspectTabBar.ItemSelected += (sender, e) => {
                if (e.Item.Tag == 1)
                    ViewModel.ShowCobuyerTab.Execute(null);
				else if (e.Item.Tag == 2)
					ViewModel.ShowTrafficCardTab.Execute(null);
            };

			var b = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				//ViewModel..Execute(null);
			});

			this.NavigationItem.SetRightBarButtonItem(b, true);

            foreach (UITabBarItem item in ProspectTabBar.Items)
            {
                if (item.Tag == 0)
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

            ProspectTabBar.SelectedItem = ProspectTabBar.Items[0];
            ProspectTabBar.ItemSelected += (sender, e) =>
            {
                if (e.Item.Tag == 1)
                    ViewModel.ShowCobuyerTab.Execute(null);
                else if (e.Item.Tag == 2)
                    ViewModel.ShowTrafficCardTab.Execute(null);
            };
        }
    }
}