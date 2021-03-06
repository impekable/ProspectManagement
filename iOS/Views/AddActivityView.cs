using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;
using Plugin.Media.Abstractions;
using Plugin.Media;
//using Microsoft.ProjectOxford.Vision;
using ProspectManagement.Core.Services;
using ProspectManagement.Core.Converters;
using ProspectManagement.iOS.Services;
using ProspectManagement.Core.Models.App;

namespace ProspectManagement.iOS.Views
{
	[MvxFromStoryboard("Main")]
	[MvxModalPresentation(WrapInNavigationController = true)]
	public partial class AddActivityView : BaseView
	{
		AlertOverlay alertOverlay;

        private TextFieldWithPopup rankingPopup;

        CognitiveVisionService cognitiveVisionService;
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

		public AddActivityView(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
            var env = new Environment_iOS();
            var theme = env.GetOperatingSystemTheme();

            base.ViewDidLoad();
			cognitiveVisionService = new CognitiveVisionService();
			var handwritingView = new HandwritingView(new System.Drawing.RectangleF(0, 0, (float)this.HandwritingContainerView.Frame.Width, (float)this.HandwritingContainerView.Frame.Height));
			handwritingView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			this.HandwritingContainerView.Add(handwritingView);

			var set = this.CreateBindingSet<AddActivityView, AddActivityViewModel>();
            CreateAlertViewBindings(set);

            set.Bind(CategoryLabel).For(v => v.Hidden).To(vm => vm.NeedCategory).WithConversion(new InverseValueConverter());
            set.Bind(RankingTextField).For(v => v.Hidden).To(vm => vm.NeedCategory).WithConversion(new InverseValueConverter());
            set.Bind(RankingTextField).To(vm => vm.ActiveRanking);
            set.Bind(NoteErrorLabel).To(vm => vm.ValidationErrorsString);
			set.Bind(NoteTextView).For(v => v.Text).To(vm => vm.Note);
			set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

			set.Apply();

            NoteTextView.Started += (sender, e) =>
            {
                if (NoteTextView.Text.Equals("Enter Note Here..."))
                {
                    AddActivityViewModel.Note = "";
                    NoteTextView.Text = "";
                }
                NoteTextView.TextColor = theme == Theme.Light ? UIColor.Black : UIColor.White; //optional
                NoteTextView.BecomeFirstResponder();
            };

            NoteTextView.Ended += (sender, e) =>
            {
                if (NoteTextView.Text.Equals(""))
                {
                    AddActivityViewModel.Note = @"Enter Note Here...";
                    NoteTextView.Text = "Enter Note Here...";
                    NoteTextView.TextColor = UIColor.LightGray; //optional
                }
                NoteTextView.ResignFirstResponder();
            };

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
			else if (AddActivityViewModel.Activity.ActivityType.Equals("ADHOC"))
			{
				this.NavigationItem.Title = "Add Follow Up Note";
			}
			else if (AddActivityViewModel.Activity.ActivityType.Equals("VISIT") || AddActivityViewModel.Activity.ActivityType.Equals("APPOINTMENT"))
			{
				this.NavigationItem.Title = "Add Visit Note";
			}

			var toolbar = new UIToolbar(new CoreGraphics.CGRect(new nfloat(0.0f), new nfloat(0.0f), this.View.Frame.Size.Width, new nfloat(44.0f)));
			toolbar.TintColor = UIColor.White;
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			var handWriteButton = new UIBarButtonItem("Write Note", UIBarButtonItemStyle.Bordered, (sender, e) =>
			{
				HandwritingContainerView.Hidden = false;
				HandwritingToolbar.Hidden = false;
				NoteTextView.ResignFirstResponder();
				NoteTextView.Hidden = true;
				handwritingView.Clear();
			});
			var photoButton = new UIBarButtonItem("Scan Note", UIBarButtonItemStyle.Bordered, async (sender, e) =>
			{
				MediaFile photo;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 3200,
                        Directory = "NoteImages",
                        Name = "Note.jpg"
                    });
                }
                else
                {
                    photo = await CrossMedia.Current.PickPhotoAsync();
                }

				if (photo != null)
				{
					using (var photoStream = photo.GetStream())
					{
						var bounds = UIScreen.MainScreen.Bounds;
						alertOverlay = new AlertOverlay(bounds, "Converting Photo to Text...");
						View.Add(alertOverlay);
						NoteTextView.Text = await cognitiveVisionService.ReadHandwrittenText(photoStream);
						AddActivityViewModel.AddAnalyticsScanPhotoCommand.Execute(null);
						alertOverlay.Hide();
					}
				}
				else
				{
					NoteTextView.BecomeFirstResponder();
				}
			});
			toolbar.SetItems(new[] { handWriteButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), photoButton}, false);
			NoteTextView.KeyboardAppearance = UIKeyboardAppearance.Default;
			NoteTextView.InputAccessoryView = toolbar;

			NoteTextView.BecomeFirstResponder();
			HandwritingContainerView.Hidden = true;
			HandwritingToolbar.Hidden = true;

			ConvertWritingBarButton.Clicked += async delegate
            {
                var img = handwritingView.GetDrawingImage();
                var jpgImage = img.AsJPEG();
                var jpgStream = jpgImage.AsStream();

				var bounds = UIScreen.MainScreen.Bounds;
                alertOverlay = new AlertOverlay(bounds, "Converting Writing to Text...");
                View.Add(alertOverlay);
				NoteTextView.Text = await cognitiveVisionService.ReadHandwrittenText(jpgStream);
				AddActivityViewModel.AddAnalyticsHandwritingCommand.Execute(null);
				alertOverlay.Hide();

				HandwritingContainerView.Hidden = true;
                HandwritingToolbar.Hidden = true;
				NoteTextView.Hidden = false;
            };

			ClearWritingBarButton.Clicked += (object sender, EventArgs e) => {
				handwritingView.Clear();
				NoteTextView.Text = null;
            };
		}

        private void CreateAlertViewBindings(MvxFluentBindingDescriptionSet<AddActivityView, AddActivityViewModel> set)
        {
            rankingPopup = new TextFieldWithPopup(RankingTextField, this);
            rankingPopup.CustomAlertController = new CustomAlertController("Category");
            set.Bind(rankingPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.Rankings);
            set.Bind(rankingPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveRanking);
            
        }
    }
}