using System;
using Cirrious.FluentLayouts.Touch;
using FFImageLoading.Cross;
using Foundation;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Converters;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Services;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    public partial class FollowUpViewCell : MvxTableViewCell
    {
        AlertOverlay alertOverlay;

        public static readonly NSString Key = new NSString("FollowUpViewCell");
        public static readonly UINib Nib;

        static FollowUpViewCell()
        {
            Nib = UINib.FromName("FollowUpViewCell", NSBundle.MainBundle);
        }

        private IMvxInteraction<String> _showAlertInteraction;
        public IMvxInteraction<String> ShowAlertInteraction
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

        private void OnShowAlertInteractionRequested(object sender, MvxValueEventArgs<String> eventArgs)
        {
            var bounds = Frame;
            alertOverlay = new AlertOverlay(bounds, eventArgs.Value);
            Superview.Add(alertOverlay);
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
            alertOverlay.Hide();
        }

        protected FollowUpViewCell(IntPtr handle) : base(handle)
        {
            //_imageControl = new MvxCachedImageView();

            //ContentView.AddSubview(_imageControl);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            SetNeedsUpdateConstraints();

            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var e = new Environment_iOS();
                var theme = e.GetOperatingSystemTheme();

                var set = this.CreateBindingSet<FollowUpViewCell, DailyToDoTaskViewModel>();
                set.Bind(ProspectLabel).To(v => v.Activity.Prospect.Name);
                set.Bind(ProspectLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme);
                set.Bind(CommunityLabel).To(v => v.Activity.Prospect.ProspectCommunity.Community.Description);
                set.Bind(CommunityLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme); 
                set.Bind(TaskLabel).To(v => v.Activity.Subject);
                set.Bind(TaskLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme);
                set.Bind(CategoryLabel).To(v => v.Activity.Prospect.Ranking);
                set.Bind(CategoryLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme);
                set.Bind(TaskDueLabel).To(v => v.Activity.TimeDateEnd).WithConversion(new DateOnlyConverter());
                set.Bind(TaskDueLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme);
                set.Bind(DayCountLabel).To(v => v.Activity.Prospect.ProspectCommunity.StartDate).WithConversion(new DayCountConverter());
                set.Bind(DayCountLabel).For(v => v.TextColor).To(v => v.Activity).WithConversion("TaskColor", theme);
                set.Bind(SMSButton).To(v => v.SmsCommand);
                set.Bind(EmailButton).To(v => v.ComposeEmailCommand);
                set.Bind(CallButton).To(v => v.PhoneCallCommand);
                set.Bind(SMSButton).For(v => v.Enabled).To(vm => vm.AllowTexting);
                set.Bind(CallButton).For(v => v.Enabled).To(vm => vm.AllowCalling);
                //set.Bind(PhoneButton).To(vm => vm.MobilePhoneCallCommand);
                set.Bind(EmailButton).For(v => v.Enabled).To(vm => vm.AllowEmailing);

                set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();
                set.Bind(this).For(view => view.ShowAlertInteraction).To(viewModel => viewModel.ShowAlertInteraction).OneWay();
                //set.Bind(this).For(u => u.BackgroundColor).To(v => v).WithConversion(new ProspectBackgroundColorValueConverter());

                //set.Bind(_imageControl).For(i => i.ImagePath).To(v => v.ProspectCommunity.SalespersonAddressNumber).WithConversion(new ImageValueConverter());
                set.Apply();
            });
        }
    }
}
