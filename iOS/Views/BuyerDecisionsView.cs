using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class BuyerDecisionsView : BaseView
    {
        AlertOverlay alertOverlay;

        private TextFieldWithPopup rankingPopup;
        private TextFieldWithPopup deactiveReasonPopup;

        protected BuyerDecisionsViewModel BuyerDecisionsViewModel => ViewModel as BuyerDecisionsViewModel;

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
            alertOverlay.Hide();
        }

        public BuyerDecisionsView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<BuyerDecisionsView, BuyerDecisionsViewModel>();
            CreateAlertViewBindings(set);

            set.Bind(DeactiveReasonTextField).To(vm => vm.CurrentDeactiveReason);
            set.Bind(DeactiveReasonTextField).For(v => v.Enabled).To(vm => vm.Deactive);
            set.Bind(RankingTextField).To(vm => vm.ActiveRanking);
            set.Bind(CategoryErrorLabel).To(vm => vm.ValidationErrorsString);
            set.Bind(UnstatisifedSwitch).To(vm => vm.BuyerDecisions.Unsatisified);
            set.Bind(HomeSwitch).To(vm => vm.BuyerDecisions.Home);
            set.Bind(MarketSwitch).To(vm => vm.BuyerDecisions.Market);
            set.Bind(BuilderSwitch).To(vm => vm.BuyerDecisions.Builder);
            set.Bind(AreaSwitch).To(vm => vm.BuyerDecisions.Area);
            set.Bind(CommunitySwitch).To(vm => vm.BuyerDecisions.Community);
            set.Bind(HomeSiteSwitch).To(vm => vm.BuyerDecisions.HomeSite);
            set.Bind(CircumstantialSwitch).To(vm => vm.BuyerDecisions.CircumstantialUrgency);
            set.Bind(EconomicClimateSwitch).To(vm => vm.BuyerDecisions.EconomicClimate);
            set.Bind(FinancingSwitch).To(vm => vm.BuyerDecisions.Financing);
            set.Bind(XFactorSwitch).To(vm => vm.BuyerDecisions.XFactor);
            set.Bind(FinalDecisionSwitch).To(vm => vm.BuyerDecisions.FinalDecision);
            set.Bind(SecondFinalDecisionSwitch).To(vm => vm.BuyerDecisions.SecondFinalDecision);

            set.Bind(SystemRankingLabel).To(vm => vm.BuyerDecisions.SystemRanking);

            set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

            set.Apply();

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                BuyerDecisionsViewModel.CloseCommand.Execute(null);
            });
            cancelButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            var saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                var bounds = UIScreen.MainScreen.Bounds;

                // show the loading overlay on the UI thread using the correct orientation sizing
                alertOverlay = new AlertOverlay(bounds, "Saving Data...");
                View.Add(alertOverlay);

                BuyerDecisionsViewModel.SaveCommand.Execute(null);

            });
            saveButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
            this.NavigationItem.SetRightBarButtonItem(saveButton, true);
            this.NavigationItem.Title = "Category";
        }

        private void CreateAlertViewBindings(MvxFluentBindingDescriptionSet<BuyerDecisionsView, BuyerDecisionsViewModel> set)
        {
            rankingPopup = new TextFieldWithPopup(RankingTextField, this);
            rankingPopup.CustomAlertController = new CustomAlertController("Category");
            set.Bind(rankingPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.Rankings);
            set.Bind(rankingPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveRanking);

            deactiveReasonPopup = new TextFieldWithPopup(DeactiveReasonTextField, this);
            deactiveReasonPopup.CustomAlertController = new CustomAlertController("Deactive Reason");
            set.Bind(deactiveReasonPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.DeactiveReasons);
            set.Bind(deactiveReasonPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.CurrentDeactiveReason);
        }
    }
}