using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Models.App;
using ProspectManagement.Core.ViewModels;
using ProspectManagement.iOS.Services;
using ProspectManagement.iOS.Utility;
using System;
using UIKit;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class CallResultView : MvxViewController<CallResultViewModel>
	{
		AlertOverlay alertOverlay;

		private TextFieldWithPopup callResultPopup;

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

		public CallResultView (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			var env = new Environment_iOS();
			var theme = env.GetOperatingSystemTheme();

			base.ViewDidLoad();

			var set = this.CreateBindingSet<CallResultView, CallResultViewModel>();
			CreateAlertViewBindings(set);

			set.Bind(CallResultTextField).To(vm => vm.CurrentCallResult);
			set.Bind(CallResultErrorLabel).To(vm => vm.ValidationErrorsString);
			set.Bind(NoteTextView).For(v => v.Text).To(vm => vm.PhoneCallActivity.Notes);
			set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

			set.Apply();

			NoteTextView.Started += (sender, e) =>
			{
				if (NoteTextView.Text.Equals("Enter Note Here..."))
				{
					ViewModel.PhoneCallActivity.Notes = "";
					NoteTextView.Text = "";
				}
				NoteTextView.TextColor = theme == Theme.Light ? UIColor.Black : UIColor.White; //optional
				NoteTextView.BecomeFirstResponder();
			};

			NoteTextView.Ended += (sender, e) =>
			{
				if (NoteTextView.Text.Equals(""))
				{
					ViewModel.PhoneCallActivity.Notes = @"Enter Note Here...";
					NoteTextView.Text = "Enter Note Here...";
					NoteTextView.TextColor = UIColor.LightGray; //optional
				}
				NoteTextView.ResignFirstResponder();
			};

			var saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				var bounds = UIScreen.MainScreen.Bounds;

				// show the loading overlay on the UI thread using the correct orientation sizing
				alertOverlay = new AlertOverlay(bounds, "Saving Data...");
				View.Add(alertOverlay);

				ViewModel.SaveCommand.Execute(null);

			});
			saveButton.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("Raleway-Bold", 18),
				TextColor = UIColor.White
			}, UIControlState.Normal);

			this.NavigationItem.SetRightBarButtonItem(saveButton, true);
            this.NavigationItem.Title = "Enter Call Results";

			if (NavigationController != null)
			{
				var stringAttributes = new UIStringAttributes();
				stringAttributes.Font = UIFont.FromName("Raleway-Bold", 20);
				stringAttributes.ForegroundColor = UIColor.FromRGB(255, 255, 255);
				NavigationController.NavigationBar.BarTintColor = ProspectManagementColors.DarkColor;
				NavigationController.NavigationBar.TintColor = UIColor.White;
				NavigationController.NavigationBar.TitleTextAttributes = stringAttributes;
				//NavigationController.NavigationBarHidden = true;
			}

			NoteTextView.BecomeFirstResponder();
		}

		private void CreateAlertViewBindings(MvxFluentBindingDescriptionSet<CallResultView, CallResultViewModel> set)
		{
			callResultPopup = new TextFieldWithPopup(CallResultTextField, this);
			callResultPopup.CustomAlertController = new CustomAlertController("Category");
			set.Bind(callResultPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.CallResults);
			set.Bind(callResultPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.CurrentCallResult);

		}
	}
}