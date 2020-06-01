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
using ProspectManagement.iOS.Services;
using MvvmCross.Base;
using ProspectManagement.Core.Models.App;
using CallKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxSplitViewPresentation(Position = MasterDetailPosition.Detail)]
    public partial class SplitDetailView : MvxViewController<SplitDetailViewModel>
    {
        private UIBarButtonItem _EditButton;
        private CustomAlertController _CallAlertController;

        private IMvxInteraction _assignedProspectInteraction;
        public IMvxInteraction AssignedProspectInteraction
        {
            get => _assignedProspectInteraction;
            set
            {
                if (_assignedProspectInteraction != null)
                    _assignedProspectInteraction.Requested -= OnAssignedProspectInteractionRequested;

                _assignedProspectInteraction = value;
                _assignedProspectInteraction.Requested += OnAssignedProspectInteractionRequested;
            }
        }

        private void OnAssignedProspectInteractionRequested(object sender, EventArgs eventArgs)
        {
            setNavigation();
        }

        private IMvxInteraction _showAlertInteraction;
        public IMvxInteraction ShowAlertInteraction
        {
            get => _showAlertInteraction;
            set
            {
                if (_showAlertInteraction != null)
                    _showAlertInteraction.Requested -= OnShowAlertInteractionRequested;

                _showAlertInteraction = value;
                _showAlertInteraction.Requested += OnShowAlertInteractionRequested;
            }
        }

        private void OnShowAlertInteractionRequested(object sender, EventArgs eventArgs)
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


        private void OnHideAlertInteractionRequested(object sender, EventArgs eventArgs)
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


        private void OnClearDetailsInteractionRequested(object sender, EventArgs eventArgs)
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

        //private IMvxInteraction<TwilioCallParameters> _makeCallInteraction;
        //public IMvxInteraction<TwilioCallParameters> MakeCallInteraction
        //{
        //    get => _makeCallInteraction;
        //    set
        //    {
        //        if (_makeCallInteraction != null)
        //            _makeCallInteraction.Requested -= OnMakeCallInteractionRequested;

        //        _makeCallInteraction = value;
        //        _makeCallInteraction.Requested += OnMakeCallInteractionRequested;
        //    }
        //}


        //private void OnMakeCallInteractionRequested(object sender, MvxValueEventArgs<TwilioCallParameters> eventArgs)
        //{
        //    var config = new CXProviderConfiguration("Call Prospect");
        //    config.MaximumCallGroups = 1;
        //    config.MaximumCallsPerCallGroup = 1;
            
        //    var callKitProvider = new CXProvider(config);
        //    var callKitController = new CXCallController();

        //    callKitProvider.SetDelegate(null, null);

        //    var twilioVoice = new TwilioVoiceHelperService();
        //    var keys = new[]
        //    {
        //        new NSString("To"),
        //        new NSString("From")
        //    };
        //    var objects = new NSString[]
        //    {
        //        new NSString(eventArgs.Value.ToPhoneNumber),
        //        new NSString(eventArgs.Value.FromPhoneNumber)
        //    };

        //    var parameters = new NSDictionary<NSString, NSString>(keys, objects);
        //    twilioVoice.MakeCall(eventArgs.Value.AccessToken, parameters);
        //}

        public SplitDetailView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SplitDetailView, SplitDetailViewModel>();
            set.Bind(NameLabel).To(vm => vm.Prospect).WithConversion(new NameConverter());
            set.Bind(AddressLine1Label).To(vm => vm.Prospect.StreetAddress.AddressLine1);
            set.Bind(CityStateZipLabel).To(vm => vm.Prospect.StreetAddress).WithConversion(new CityStateZipConverter());

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

            set.Bind(MessageButton).To(vm => vm.SMSCommand);
            set.Bind(MessageButton).For(v => v.Enabled).To(vm => vm.AllowTexting);
            set.Bind(MessageButton).For(v => v.Hidden).To(vm => vm.User.UsingTelephony).WithConversion(new InverseValueConverter());
            set.Bind(MessageLabel).For(v => v.Hidden).To(vm => vm.User.UsingTelephony).WithConversion(new InverseValueConverter());
            set.Bind(PhoneButton).For(v => v.Enabled).To(vm => vm.AllowCalling);
            //set.Bind(PhoneButton).To(vm => vm.MobilePhoneCallCommand);
            set.Bind(EmailButton).For(v => v.Enabled).To(vm => vm.AllowEmailing);
            set.Bind(EmailButton).To(vm => vm.ComposeEmailCommand);
            //set.Bind(RankingButton).To(vm => vm.ShowRankingCommand);

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
            //set.Bind(this).For(view => view.MakeCallInteraction).To(viewModel => viewModel.MakeCallInteraction).OneWay();

            _CallAlertController = new CustomAlertController("Call");
            set.Bind(_CallAlertController).For(p => p.AlertController).To(vm => vm.Phones);
            set.Bind(_CallAlertController).For(p => p.SelectedCode).To(vm => vm.SelectedCall);

            set.Apply();

            ProspectTabBar.SelectedItem = ProspectTabBar.Items[0];
            ProspectTabBar.ItemSelected += (sender, e) =>
            {
                if (e.Item.Tag == 1)
                    ViewModel.ShowCobuyerTab.Execute(null);
                else if (e.Item.Tag == 2)
                    ViewModel.ShowTrafficCardTab.Execute(null);
                else if (e.Item.Tag == 3)
                    ViewModel.ShowContactHistoryTab.Execute(null);
                else if (e.Item.Tag == 4)
                    ViewModel.ShowRankingCommand.Execute(null);
            };

            setNavigation();

            PhoneButton.TouchUpInside += (sender, e) =>
            {
                var popPresenter = _CallAlertController.AlertController.PopoverPresentationController;
                if (popPresenter != null)
                {
                    popPresenter.SourceView = PhoneButton;
                    popPresenter.SourceRect = PhoneButton.Bounds;
                }
                PresentViewController(_CallAlertController.AlertController, true, null);
            };
        }

        private void setNavigation()
        {
            foreach (UITabBarItem item in ProspectTabBar.Items)
            {
                ProspectTabBar.Items[item.Tag].Enabled = true;

                if (item.Tag == 0)
                {
                    item.SetTitleTextAttributes(new UITextAttributes()
                    {
                        Font = UIFont.FromName("Raleway-Bold", 10),
                        TextColor = ProspectManagementColors.LabelColor
                    }, UIControlState.Normal);

                }
                else
                {
                    item.SetTitleTextAttributes(new UITextAttributes()
                    {
                        Font = UIFont.FromName("Raleway-Regular", 10),
                        TextColor = ProspectManagementColors.LabelColor
                    }, UIControlState.Normal);
                }

                if (ViewModel.IsLead && (item.Tag == 1 || item.Tag == 2 || item.Tag == 4))
                {
                    ProspectTabBar.Items[item.Tag].Enabled = false;
                }
            }

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

                ProspectTabBar.Hidden = false;
            }
            else
            {
                if (!ViewModel.IsLead)
                    ProspectTabBar.Hidden = true;
            }
        }
    }
}
