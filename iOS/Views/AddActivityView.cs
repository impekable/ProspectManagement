using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class AddActivityView : BaseView
    {
        AlertOverlay alertOverlay;

        protected AddActivityViewModel AddActivityViewModel => ViewModel as AddActivityViewModel;

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

        public AddActivityView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<AddActivityView, AddActivityViewModel>();

            set.Bind(NoteErrorLabel).To(vm => vm.NoteError);
            set.Bind(NoteTextView).For(v => v.Text).To(vm => vm.Note);
            set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

            set.Apply();

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                AddActivityViewModel.CloseCommand.Execute(null);
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

                AddActivityViewModel.SaveCommand.Execute(null);

            });
            saveButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
            this.NavigationItem.SetRightBarButtonItem(saveButton, true);


            if (AddActivityViewModel.Activity.ActivityType.Equals("COMMENT"))
            {
                this.NavigationItem.Title = "Add Note";
            }
            else if (AddActivityViewModel.Activity.ActivityType.Equals("VISIT") || AddActivityViewModel.Activity.ActivityType.Equals("APPOINTMENT"))
            {
                this.NavigationItem.Title = "Add Visit Note";
            }

            NoteTextView.BecomeFirstResponder();
        }

    }
}