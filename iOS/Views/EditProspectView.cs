using Foundation;
using System;
using UIKit;
using ProspectManagement.Core.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;
using System.Linq;
using MvvmCross.Binding.iOS.Views;
using ProspectManagement.Core.Converters;
using CoreGraphics;
using ProspectManagement.iOS.Utility;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Core;
using ProspectManagement.iOS.Extensions;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class EditProspectView : BaseView
    {
        private UIPickerView _defaultPrefixPickerView;
        private MvxPickerViewModel _defaultPrefixPickerViewModel;
		private UIPickerView _defaultCountryPickerView;
		private MvxPickerViewModel _defaultCountryPickerViewModel;
        private UIPickerView _defaultSuffixPickerView;
        private MvxPickerViewModel _defaultSuffixPickerViewModel;
        private UIPickerView _defaultContactPreferencePickerView;
        private MvxPickerViewModel _defaultContactPreferencePickerViewModel;
        private UIPickerView _defaultStatePickerView;
        private MvxPickerViewModel _defaultStatePickerViewModel;
        private UIPickerView _defaultExcludeReasonPickerView;
        private MvxPickerViewModel _defaultExcludeReasonPickerViewModel;
        private UIPickerView _defaultTrafficSourcePickerView;
        private MvxPickerViewModel _defaultTrafficSourcePickerViewModel;
        private UIPickerView _defaultTrafficDetailPickerView;
        private MvxPickerViewModel _defaultTrafficDetailPickerViewModel;

        private static readonly int TextFieldMargin = 10;
        public static readonly int ArrowWidth = 14;
        protected EditProspectViewModel EditProspectViewModel => ViewModel as EditProspectViewModel;

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
            var scrollView = ValidationErrorsLabel.FindSuperviewOfType(View, typeof(UIScrollView)) as UIScrollView;
            RestoreScrollPosition(scrollView, 100);
		}

        public EditProspectView(IntPtr handle) : base(handle)
        {
            ScrollViewInset = 80;
        }

        public override bool HandlesKeyboardNotifications()
        {
            return true;
        }

        UITextFieldCondition shouldReturn = (textField) =>
        {
            //UIView baseView = textField.Superview;
            //while (baseView.Superview != null)
                //baseView = baseView.Superview;

            //var nextField = baseView.ViewWithTag(textField.Tag + 1) as UITextField;
            //if (nextField != null)
            //{
            //    nextField.BecomeFirstResponder();
            //}
            //else
            {
                textField.ResignFirstResponder();
            }
            return false;
        };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<EditProspectView, EditProspectViewModel>();
            CreatePickerViewBindings(set);

            set.Bind(PrefixTextField).To(vm => vm.ActivePrefix.Description1);
            set.Bind(FirstNameTextField).To(vm => vm.FirstName);
            set.Bind(MiddleNameTextField).To(vm => vm.MiddleName);
            set.Bind(LastNameTextField).To(vm => vm.LastName);
            set.Bind(SuffixTextField).To(vm => vm.ActiveSuffix.Description1);
            set.Bind(AliasTextField).To(vm => vm.NickName);
            set.Bind(MobilePhoneTextField).To(vm => vm.MobilePhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(WorkPhoneTextField).To(vm => vm.WorkPhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(HomePhoneTextField).To(vm => vm.HomePhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(WorkExtentionTextField).To(vm => vm.WorkPhone.PhoneExtension);
            set.Bind(EmailTextField).To(vm => vm.Email.EmailAddress);

            set.Bind(AddressLine1TextField).To(vm => vm.StreetAddress.AddressLine1);
            set.Bind(AddressLine2TextField).To(vm => vm.StreetAddress.AddressLine2);
            set.Bind(CityTextField).To(vm => vm.StreetAddress.City);
            set.Bind(StateTextField).To(vm => vm.ActiveState.Description1);
            set.Bind(CountyTextField).To(vm => vm.StreetAddress.County);
            set.Bind(ZipTextField).To(vm => vm.StreetAddress.PostalCode);
            set.Bind(CountryTextField).To(vm => vm.ActiveCountry.Description1);
            set.Bind(CountyLabel).To(vm => vm.ActiveCountry).WithConversion(new CountyConverter());
            set.Bind(StateCountryLabel).To(vm => vm.ActiveCountry).WithConversion(new StateCountryConverter());

            set.Bind(ConsentEmailSwitch).To(vm => vm.FollowUpSettings.ConsentToEmail);
            set.Bind(ConsentPhoneSwitch).To(vm => vm.FollowUpSettings.ConsentToPhone);
            set.Bind(ConsentMailSwitch).To(vm => vm.FollowUpSettings.ConsentToMail);
            set.Bind(ExcludeFollowUpSwitch).To(vm => vm.ExcludeFromFollowup);

            set.Bind(FirstNameErrorLabel).To(vm => vm.FirstNameError);
            set.Bind(LastNameErrorLabel).To(vm => vm.LastNameError);
            set.Bind(HomePhoneErrorLabel).To(vm => vm.HomePhoneNumberError);
            set.Bind(WorkPhoneErrorLabel).To(vm => vm.WorkPhoneNumberError);
            set.Bind(MobilePhoneErrorLabel).To(vm => vm.MobilePhoneNumberError);
            set.Bind(EmailErrorLabel).To(vm => vm.EmailAddressError);
            set.Bind(TrafficSourceDetailError).To(vm => vm.TrafficSourceDetailError);
            set.Bind(ValidationErrorsLabel).To(vm => vm.ValidationErrorsString);

            set.Bind(TrafficDetailTextField).To(vm => vm.Prospect.TrafficSourceCodeId).OneWayToSource();
            set.Bind(TrafficDetailTextField).For(v => v.Enabled).To(vm => vm.IsSelectedTrafficSource);
            set.Bind(ExcludeReasonTextField).For(v => v.Enabled).To(vm => vm.ExcludeFromFollowup);
			set.Bind(StateTextField).For(v => v.Hidden).To(vm => vm.ForeignState);

            set.Bind(TrafficSourceLabel).For(v => v.Hidden).To(vm => vm.FromLead);
            set.Bind(TrafficSourceDetailLabel).For(v => v.Hidden).To(vm => vm.FromLead);
            set.Bind(TrafficDetailTextField).For(v => v.Hidden).To(vm => vm.FromLead);
            set.Bind(TrafficSourceTextField).For(v => v.Hidden).To(vm => vm.FromLead);
			set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

			CustomizeTextField(PrefixTextField, _defaultPrefixPickerView, "Prefix");
            CustomizeTextField(SuffixTextField, _defaultSuffixPickerView, "Suffix");
            CustomizeTextField(StateTextField, _defaultStatePickerView, "State");
			CustomizeTextField(CountryTextField, _defaultCountryPickerView, "Country");
            CustomizeTextField(ContactPreferenceTextField, _defaultContactPreferencePickerView, "ContactPreference");
            CustomizeTextField(ExcludeReasonTextField, _defaultExcludeReasonPickerView, "ExcludeReason");
            CustomizeTextField(TrafficSourceTextField, _defaultTrafficSourcePickerView, "Source");
            CustomizeTextField(TrafficDetailTextField, _defaultTrafficDetailPickerView, "Detail");

            set.Apply();

            ConsentPhoneSwitch.On = EditProspectViewModel.Prospect.FollowUpSettings.ConsentToPhone;

            PrefixTextField.ShouldReturn = shouldReturn;
            FirstNameTextField.ShouldReturn = shouldReturn;
            MiddleNameTextField.ShouldReturn = shouldReturn;
            LastNameTextField.ShouldReturn = shouldReturn;
            SuffixTextField.ShouldReturn = shouldReturn;
            AliasTextField.ShouldReturn = shouldReturn;
            EmailTextField.ShouldReturn = shouldReturn;
            MobilePhoneTextField.ShouldReturn = shouldReturn;
            WorkPhoneTextField.ShouldReturn = shouldReturn;
            WorkExtentionTextField.ShouldReturn = shouldReturn;
            HomePhoneTextField.ShouldReturn = shouldReturn;

            AddressLine1TextField.ShouldReturn = shouldReturn;
            AddressLine2TextField.ShouldReturn = shouldReturn;
            CityTextField.ShouldReturn = shouldReturn;
            StateTextField.ShouldReturn = shouldReturn;
            CountryTextField.ShouldReturn = shouldReturn;
            CountyTextField.ShouldReturn = shouldReturn;
            ZipTextField.ShouldReturn = shouldReturn;


            PrefixTextField.EditingDidBegin += (sender, e) =>
            {
                EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActivePrefix;
                if (EditProspectViewModel.ActivePrefix == null)
                    EditProspectViewModel.ActivePrefix = EditProspectViewModel.Prefixes != null ? EditProspectViewModel.Prefixes.FirstOrDefault(): null;
            };
            SuffixTextField.EditingDidBegin += (sender, e) =>
            {
                EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveSuffix;
				if (EditProspectViewModel.ActiveSuffix == null)
					EditProspectViewModel.ActiveSuffix = EditProspectViewModel.Suffixes.FirstOrDefault();
            };
            StateTextField.EditingDidBegin += (sender, e) =>
            {
                EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveState;
				if (EditProspectViewModel.ActiveState == null)
					EditProspectViewModel.ActiveState = EditProspectViewModel.States.FirstOrDefault();
            };
			CountryTextField.EditingDidBegin += (sender, e) =>
			{
				EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveCountry;
				if (EditProspectViewModel.ActiveCountry == null)
					EditProspectViewModel.ActiveCountry = EditProspectViewModel.Countries.FirstOrDefault();
			};
            ContactPreferenceTextField.EditingDidBegin += (sender, e) =>
            {
                EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveContactPreference;
				if (EditProspectViewModel.ActiveContactPreference == null)
					EditProspectViewModel.ActiveContactPreference = EditProspectViewModel.ContactPreferences.FirstOrDefault();
            };
            ExcludeReasonTextField.EditingDidBegin += (sender, e) =>
            {
            	EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveExcludeReason;
				if (EditProspectViewModel.ActiveExcludeReason == null)
					EditProspectViewModel.ActiveExcludeReason = EditProspectViewModel.ExcludeReasons.FirstOrDefault();
            };
			TrafficSourceTextField.EditingDidBegin += (sender, e) =>
			{
				EditProspectViewModel.OriginalTrafficSource = EditProspectViewModel.ActiveTrafficSource;
				if (EditProspectViewModel.ActiveTrafficSource == null)
					EditProspectViewModel.ActiveTrafficSource = EditProspectViewModel.TrafficSources.FirstOrDefault();
			};

			TrafficDetailTextField.EditingDidBegin += (sender, e) =>
			{
				EditProspectViewModel.OriginalTrafficSourceDetail = EditProspectViewModel.ActiveTrafficSourceDetail;
				if (EditProspectViewModel.ActiveTrafficSourceDetail == null)
					EditProspectViewModel.ActiveTrafficSourceDetail = EditProspectViewModel.ActiveTrafficSource.TrafficSourceDetails.First();
			};

			var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				EditProspectViewModel.CloseCommand.Execute(null);
			});

			var saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				UIAlertController myAlert = UIAlertController.Create("Saving", "", UIAlertControllerStyle.Alert);
				var activity = new UIActivityIndicatorView() { ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge };
				activity.Frame = myAlert.View.Bounds;
				activity.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
				activity.Color = UIColor.Black;
				activity.StartAnimating();
				myAlert.Add(activity);
				this.PresentViewController(myAlert, true, null);

				EditProspectViewModel.SaveCommand.Execute(null);

			});

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
			this.NavigationItem.SetRightBarButtonItem(saveButton, true);
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("Raleway-Bold", 20)
			});
            this.NavigationItem.Title = "Edit Prospect";
        }

        private void CustomizeTextField(UITextField textField, UIPickerView pickerView, string pickerType)
        {
            textField.TintColor = UIColor.Clear;

            //var leftPaddingView = new UIView(new CGRect(0, 0, TextFieldMargin, textField.Frame.Height));
            var arrowImageView = new UIImageView(new CGRect(0, 0, ArrowWidth + TextFieldMargin, textField.Frame.Height))
            {
                Image = UIImage.FromBundle("arrow-down.png"),
                ContentMode = UIViewContentMode.Left
            };
            textField.RightView = arrowImageView;
            textField.RightViewMode = UITextFieldViewMode.UnlessEditing;

            textField.InputView = pickerView;
            var toolbar = CreateToolbar(textField, pickerView, pickerType);
            textField.InputAccessoryView = toolbar;
            textField.Layer.BorderColor = ProspectManagementColors.BorderColor.CGColor;
            textField.Layer.BorderWidth = 1;
        }

        private UIToolbar CreateToolbar(UITextField textField, UIPickerView pickerView, string pickerType)
        {
			//UIView baseView = textField.Superview;
			//while (baseView.Superview != null)
			//	baseView = baseView.Superview;

			//var nextField = baseView.ViewWithTag(textField.Tag + 1) as UITextField;

            var toolbar = new UIToolbar(new CGRect(0, 0, 320, 44));
            var cancel = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered, (sender, e) =>
            {
                var row = 0;
                switch (pickerType)
                {
                    case "State":
                        {
                            row = EditProspectViewModel.States.IndexOf(EditProspectViewModel.OriginalPickerValue);
                            EditProspectViewModel.ActiveState = null; //EditProspectViewModel.OriginalPickerValue;
							break;
                        };
					case "Country":
						{
							row = EditProspectViewModel.Countries.IndexOf(EditProspectViewModel.OriginalPickerValue);
							EditProspectViewModel.ActiveCountry = null; //EditProspectViewModel.OriginalPickerValue;
							break;
						};
                    case "Suffix":
                        {
                            row = EditProspectViewModel.Suffixes.IndexOf(EditProspectViewModel.OriginalPickerValue);
                            EditProspectViewModel.ActiveSuffix = null;// EditProspectViewModel.OriginalPickerValue;
                            break;
                        };
                    case "Prefix":
                        {
                            row = EditProspectViewModel.Prefixes.IndexOf(EditProspectViewModel.OriginalPickerValue);
                            EditProspectViewModel.ActivePrefix = null; //EditProspectViewModel.OriginalPickerValue;
							break;
                        };
                    case "ContactPreference":
                        {
                            row = EditProspectViewModel.ContactPreferences.IndexOf(EditProspectViewModel.OriginalPickerValue);
                            EditProspectViewModel.ActiveContactPreference = EditProspectViewModel.OriginalPickerValue;
                            break;
                        };
					case "ExcludeReason":
						{
							row = EditProspectViewModel.ExcludeReasons.IndexOf(EditProspectViewModel.OriginalPickerValue);
							EditProspectViewModel.ActiveExcludeReason = null;//EditProspectViewModel.OriginalPickerValue;
							break;
						};
                    case "Source":
                        {
                            row = EditProspectViewModel.TrafficSources.IndexOf(EditProspectViewModel.ActiveTrafficSource);
                            EditProspectViewModel.ActiveTrafficSource = EditProspectViewModel.OriginalTrafficSource;
                            break;
                        };
                    case "Detail":
                        {
                            row = EditProspectViewModel.ActiveTrafficSource.TrafficSourceDetails.IndexOf(EditProspectViewModel.ActiveTrafficSourceDetail);
                            EditProspectViewModel.ActiveTrafficSourceDetail = EditProspectViewModel.OriginalTrafficSourceDetail;
                            break;
                        };
                    default: { break; };
                }
                pickerView.Select(row, 0, true);
				//if (nextField != null)
				//{
				//	nextField.BecomeFirstResponder();
				//}
				//else
				{
					textField.ResignFirstResponder();
				}
            })
            {
                TintColor = ProspectManagementColors.AccentColor
            };

            var done = new UIBarButtonItem("Done", UIBarButtonItemStyle.Bordered, (sender, e) =>
            {
                //if (nextField != null)
                //{
                //    nextField.BecomeFirstResponder();
                //}
                //else
                {
                    textField.ResignFirstResponder();
                }
            })
            {
                TintColor = ProspectManagementColors.AccentColor
            };
            toolbar.SetItems(new[] { cancel, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), done }, false);
            return toolbar;
        }

        private void CreatePickerViewBindings(MvxFluentBindingDescriptionSet<EditProspectView, EditProspectViewModel> set)
        {
            set.Bind(ContactPreferenceTextField).To(vm => vm.ActiveContactPreference.Description1).OneWay();
            _defaultContactPreferencePickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultContactPreferencePickerViewModel = new CustomPickerView(_defaultContactPreferencePickerView);
            _defaultContactPreferencePickerView.Model = _defaultContactPreferencePickerViewModel;
            set.Bind(_defaultContactPreferencePickerViewModel).For(p => p.ItemsSource).To(vm => vm.ContactPreferences).OneWay();
            set.Bind(_defaultContactPreferencePickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveContactPreference).OneWayToSource();


            set.Bind(StateTextField).To(vm => vm.ActiveState.Description1).OneWay();
            _defaultStatePickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultStatePickerViewModel = new CustomPickerView(_defaultStatePickerView);
            _defaultStatePickerView.Model = _defaultStatePickerViewModel;
            set.Bind(_defaultStatePickerViewModel).For(p => p.ItemsSource).To(vm => vm.States).OneWay();
            set.Bind(_defaultStatePickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveState).OneWayToSource();


			set.Bind(CountryTextField).To(vm => vm.ActiveCountry.Description1).OneWay();
			_defaultCountryPickerView = new UIPickerView
			{
				ShowSelectionIndicator = true
			};
			_defaultCountryPickerViewModel = new CustomPickerView(_defaultCountryPickerView);
			_defaultCountryPickerView.Model = _defaultCountryPickerViewModel;
			set.Bind(_defaultCountryPickerViewModel).For(p => p.ItemsSource).To(vm => vm.Countries).OneWay();
			set.Bind(_defaultCountryPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveCountry).OneWayToSource();

			set.Bind(ExcludeReasonTextField).To(vm => vm.ActiveExcludeReason.Description1).OneWay();
            _defaultExcludeReasonPickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultExcludeReasonPickerViewModel = new CustomPickerView(_defaultExcludeReasonPickerView);
            _defaultExcludeReasonPickerView.Model = _defaultExcludeReasonPickerViewModel;
            set.Bind(_defaultExcludeReasonPickerViewModel).For(p => p.ItemsSource).To(vm => vm.ExcludeReasons).OneWay();
            set.Bind(_defaultExcludeReasonPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveExcludeReason).OneWayToSource();


            set.Bind(SuffixTextField).To(vm => vm.ActiveSuffix.Description1).OneWay();
            _defaultSuffixPickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultSuffixPickerViewModel = new CustomPickerView(_defaultSuffixPickerView);
            _defaultSuffixPickerView.Model = _defaultSuffixPickerViewModel;
            set.Bind(_defaultSuffixPickerViewModel).For(p => p.ItemsSource).To(vm => vm.Suffixes).OneWay();
            set.Bind(_defaultSuffixPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveSuffix).OneWayToSource();


            set.Bind(PrefixTextField).To(vm => vm.ActivePrefix.Description1).OneWay();
            _defaultPrefixPickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultPrefixPickerViewModel = new CustomPickerView(_defaultPrefixPickerView);
            _defaultPrefixPickerView.Model = _defaultPrefixPickerViewModel;
            set.Bind(_defaultPrefixPickerViewModel).For(p => p.ItemsSource).To(vm => vm.Prefixes).OneWay();
            set.Bind(_defaultPrefixPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActivePrefix).OneWayToSource();


            set.Bind(TrafficSourceTextField).To(vm => vm.ActiveTrafficSource.SourceDescription).OneWay();
            _defaultTrafficSourcePickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultTrafficSourcePickerViewModel = new CustomPickerView(_defaultTrafficSourcePickerView);
            _defaultTrafficSourcePickerView.Model = _defaultTrafficSourcePickerViewModel;
            set.Bind(_defaultTrafficSourcePickerViewModel).For(p => p.ItemsSource).To(vm => vm.TrafficSources).OneWay();
            set.Bind(_defaultTrafficSourcePickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveTrafficSource).OneWayToSource();


            set.Bind(TrafficDetailTextField).To(vm => vm.ActiveTrafficSourceDetail.CodeDescription).OneWay();
            _defaultTrafficDetailPickerView = new UIPickerView
            {
                ShowSelectionIndicator = true
            };
            _defaultTrafficDetailPickerViewModel = new CustomPickerView(_defaultTrafficDetailPickerView);
            _defaultTrafficDetailPickerView.Model = _defaultTrafficDetailPickerViewModel;
            set.Bind(_defaultTrafficDetailPickerViewModel).For(p => p.ItemsSource).To(vm => vm.ActiveTrafficSource.TrafficSourceDetails).OneWay();
            set.Bind(_defaultTrafficDetailPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveTrafficSourceDetail).OneWayToSource();

        }
    }
}