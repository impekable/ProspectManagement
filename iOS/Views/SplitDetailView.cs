using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Converters;
using ProspectManagement.iOS.Utility;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
	[MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
    public partial class SplitDetailView : MvxViewController<SplitDetailViewModel>
    {
        protected SplitDetailViewModel SplitDetailViewModel => ViewModel as SplitDetailViewModel;
        private UIBarButtonItem _EditButton;

        private IMvxInteraction _assignedProspectInteraction;
        public IMvxInteraction AssignedProspectInteraction
        {
            get => _hideAlertInteraction;
            set
            {
                if (_assignedProspectInteraction != null)
                    _assignedProspectInteraction.Requested -= OnAssignedProspectInteractionRequested;

                _assignedProspectInteraction = value;
                _assignedProspectInteraction.Requested += OnAssignedProspectInteractionRequested;
            }
        }

        private async void OnAssignedProspectInteractionRequested(object sender, EventArgs eventArgs)
        {
            setNavigation();
        }

        private IMvxInteraction _showAlertInteraction;
        public IMvxInteraction ShowAlertInteraction
        {
            get => _hideAlertInteraction;
            set
            {
                if (_showAlertInteraction != null)
                    _showAlertInteraction.Requested -= OnShowAlertInteractionRequested;

                _showAlertInteraction = value;
                _showAlertInteraction.Requested += OnShowAlertInteractionRequested;
            }
        }

        private async void OnShowAlertInteractionRequested(object sender, EventArgs eventArgs)
        {
            UIAlertController myAlert = UIAlertController.Create("", "", UIAlertControllerStyle.Alert);
            var activity = new UIActivityIndicatorView() { ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge };
            activity.Frame = myAlert.View.Bounds;
            activity.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            activity.Color = UIColor.Black;
            activity.StartAnimating();
            myAlert.Add(activity);
            this.PresentViewController(myAlert, true, null);

        }

        private IMvxInteraction _hideAlertInteraction;
        public IMvxInteraction HideAlertInteraction
        {
            get => _hideAlertInteraction;
            set
            {
                if (_hideAlertInteraction != null)
                    _hideAlertInteraction.Requested -= OnHideAlertInteractionRequested;

                _hideAlertInteraction = value;
                _hideAlertInteraction.Requested += OnHideAlertInteractionRequested;
            }
        }


        private async void OnHideAlertInteractionRequested(object sender, EventArgs eventArgs)
        {
            DismissViewController(true, null);
        }

        private IMvxInteraction _clearDetailsInteraction;
        public IMvxInteraction ClearDetailsInteraction
        {
            get => _clearDetailsInteraction;
            set
            {
                if (_clearDetailsInteraction != null)
                    _clearDetailsInteraction.Requested -= OnClearDetailsInteractionRequested;

                _clearDetailsInteraction = value;
                _clearDetailsInteraction.Requested += OnClearDetailsInteractionRequested;
            }
        }


        private async void OnClearDetailsInteractionRequested(object sender, EventArgs eventArgs)
        {
            DetailStackView.Hidden = true;
            ProspectTabBar.Hidden = true;
            if (_EditButton != null)
            {
                _EditButton.Enabled = false;
                _EditButton.Title = "";
            }
            this.NavigationItem.Title = "";
        }

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

			set.Bind(ComposeEmailButton).For(v => v.Enabled).To(vm => vm.AllowEmailing);
            set.Bind(ComposeEmailButton).For("Title").To(vm => vm.Prospect.Email.EmailAddress);
			set.Bind(ComposeEmailButton).To(vm => vm.ComposeEmailCommand);

			set.Bind(CallWorkButton).For(v => v.Enabled).To(vm => vm.AllowCalling);
			set.Bind(CallWorkButton).For("Title").To(vm => vm.Prospect.WorkPhoneNumber.Phone);
			set.Bind(CallWorkButton).To(vm => vm.WorkPhoneCallCommand);
            set.Bind(WorkLabel).To(vm => vm.WorkPhoneLabel);

			set.Bind(CallHomeButton).For(v => v.Enabled).To(vm => vm.AllowCalling);
			set.Bind(CallHomeButton).For("Title").To(vm => vm.Prospect.HomePhoneNumber.Phone);
			set.Bind(CallHomeButton).To(vm => vm.HomePhoneCallCommand);
            set.Bind(HomeLabel).To(vm => vm.HomePhoneLabel);

			set.Bind(CallMobileButton).For(v => v.Enabled).To(vm => vm.AllowCalling);
			set.Bind(CallMobileButton).For("Title").To(vm => vm.Prospect.MobilePhoneNumber.Phone);
			set.Bind(CallMobileButton).To(vm => vm.MobilePhoneCallCommand);
			set.Bind(MobileLabel).To(vm => vm.MobilePhoneLabel);

            set.Bind(ConsentLabel).To(vm => vm.Prospect.FollowUpSettings).WithConversion(new FollowUpSettingsValueConverter());
            set.Bind(ContactPreferenceLabel).To(vm => vm.Prospect.FollowUpSettings.PreferredContactMethod);
			set.Bind(AppointmentDateTimeLabel).To(vm => vm.Prospect.ProspectCommunity.AppointmentDate).WithConversion(new DateTimeConverter());
            
            set.Bind(AssignButton).For(c => c.Hidden).To(vm => vm.Assigned);
            set.Bind(AssignButton).To(vm => vm.AssignCommand);
			set.Bind(AssignButton).For("Title").To(vm => vm.AssignText);

            set.Bind(AddNoteButton).To(vm => vm.AddNoteCommand);
            set.Bind(CompleteApptButton).To(vm => vm.CompleteApptCommand);
            set.Bind(AddVisitButton).To(vm => vm.AddVisitCommand);

            set.Bind(AddNoteButton).For(c => c.Hidden).To(vm => vm.AssignedProspect).WithConversion(new InverseValueConverter());
            set.Bind(CompleteApptButton).For(v => v.Hidden).To(vm => vm.IsLeadWithAppointment).WithConversion(new InverseValueConverter());
            set.Bind(AddVisitButton).For(v => v.Hidden).To(vm => vm.AssignedWithoutAppointment).WithConversion(new InverseValueConverter());

            set.Bind(StreetAddressStackView).For(v => v.Hidden).To(vm => vm.StreetAddressEntered).WithConversion(new InverseValueConverter());
            set.Bind(EmailStackView).For(v => v.Hidden).To(vm => vm.EmailEntered).WithConversion(new InverseValueConverter());
            set.Bind(WorkStackView).For(v => v.Hidden).To(vm => vm.WorkPhoneEntered).WithConversion(new InverseValueConverter());
            set.Bind(HomeStackView).For(v => v.Hidden).To(vm => vm.HomePhoneEntered).WithConversion(new InverseValueConverter());
            set.Bind(MobileStackView).For(v => v.Hidden).To(vm => vm.MobilePhoneEntered).WithConversion(new InverseValueConverter());

            set.Bind(ExcludeStackView).For(v => v.Hidden).To(vm => vm.Prospect.FollowUpSettings.ExcludeFromFollowup).WithConversion(new InverseValueConverter());
            set.Bind(ConsentStackView).For(v => v.Hidden).To(vm => vm.Prospect.FollowUpSettings.ExcludeFromFollowup);
			set.Bind(AppointmentStackView).For(v => v.Hidden).To(vm => vm.IsLeadWithAppointment).WithConversion(new InverseValueConverter());
            
            set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();
            set.Bind(this).For(view => view.ShowAlertInteraction).To(viewModel => viewModel.ShowAlertInteraction).OneWay();
            set.Bind(this).For(view => view.ClearDetailsInteraction).To(viewModel => viewModel.ClearDetailsInteraction).OneWay();
            set.Bind(this).For(view => view.AssignedProspectInteraction).To(viewModel => viewModel.AssignedProspectInteraction).OneWay();
            set.Apply();

            setNavigation();
        }

        private void setNavigation()
        {
            if (ViewModel.AssignedProspect)
            {
                _EditButton = new UIBarButtonItem("Edit", UIBarButtonItemStyle.Plain, (sender, e) =>
                {
                    ViewModel.EditProspectCommand.Execute(null);
                });
                _EditButton.SetTitleTextAttributes(new UITextAttributes()
                {
                    Font = UIFont.FromName("Raleway-Bold", 18),
                    TextColor = ProspectManagementColors.DarkColor
                }, UIControlState.Normal);

                this.NavigationItem.SetRightBarButtonItem(_EditButton, true);

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

                ProspectTabBar.Hidden = false;
                ProspectTabBar.SelectedItem = ProspectTabBar.Items[0];
                ProspectTabBar.ItemSelected += (sender, e) =>
                {
                    if (e.Item.Tag == 1)
                        ViewModel.ShowCobuyerTab.Execute(null);
                    else if (e.Item.Tag == 2)
                        ViewModel.ShowTrafficCardTab.Execute(null);
                };
            }
            else
            {
                ProspectTabBar.Hidden = true;
            }
        }
    }
}