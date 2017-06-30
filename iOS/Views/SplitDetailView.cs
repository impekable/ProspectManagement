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
            set.Bind(ProspectTabBar).For(c => c.Hidden).To(vm => vm.ProspectSelected).WithConversion(new InverseValueConverter());
            set.Apply();

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

            //UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            //{
            //    Font = UIFont.FromName("Raleway-Bold", 20)
            //});
        }
    }
}