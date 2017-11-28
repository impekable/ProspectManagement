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
using ProspectManagement.Core.Models;
using System.Threading.Tasks;
using CoreFoundation;
using System.Collections.ObjectModel;

namespace ProspectManagement.iOS.Views
{
    [MvxFromStoryboard("Main")]
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class EditProspectView : BaseView
    {
        AlertOverlay alertOverlay;
        private TextFieldWithPopup prefixPopup;
        private TextFieldWithPopup suffixPopup;
        private TextFieldWithPopup statePopup;
        private TextFieldWithPopup preferencePopup;
        private TextFieldWithPopup excludePopup;
        private TextFieldWithPopup trafficSourcePopup;
        private TextFieldWithPopup trafficDetailPopup;

        private UIPickerView _defaultCountryPickerView;
        private MvxPickerViewModel _defaultCountryPickerViewModel;

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
            alertOverlay.Hide();
        }

        public EditProspectView(IntPtr handle) : base(handle)
        {
            ScrollViewInset = 0;
        }

        public override bool HandlesKeyboardNotifications()
        {
            return true;
        }

        UITextFieldCondition shouldReturn = (textField) =>
        {
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
            CreateAlertViewBindings(set);

            set.Bind(PrefixTextField).To(vm => vm.ActivePrefix.Description1);
            set.Bind(FirstNameTextField).To(vm => vm.FirstName).WithConversion(new TitleCaseValueConverter());
            set.Bind(MiddleNameTextField).To(vm => vm.MiddleName).WithConversion(new TitleCaseValueConverter());;
            set.Bind(LastNameTextField).To(vm => vm.LastName).WithConversion(new TitleCaseValueConverter());;
            set.Bind(SuffixTextField).To(vm => vm.ActiveSuffix.Description1);
            set.Bind(AliasTextField).To(vm => vm.NickName).WithConversion(new TitleCaseValueConverter());;
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

            set.Bind(ContactPreferenceTextField).To(vm => vm.ActiveContactPreference.Description1);
            set.Bind(ConsentEmailSwitch).To(vm => vm.FollowUpSettings.ConsentToEmail);
            set.Bind(ConsentPhoneSwitch).To(vm => vm.FollowUpSettings.ConsentToPhone);
            set.Bind(ConsentMailSwitch).To(vm => vm.FollowUpSettings.ConsentToMail);
            set.Bind(ExcludeFollowUpSwitch).To(vm => vm.ExcludeFromFollowup);
            set.Bind(ExcludeReasonTextField).To(vm => vm.ActiveExcludeReason.Description1);

            set.Bind(TrafficSourceTextField).To(vm => vm.ActiveTrafficSource.SourceDescription);
            set.Bind(TrafficDetailTextField).To(vm => vm.ActiveTrafficSourceDetail.CodeDescription);

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

            CustomizeTextFieldWithPicker(CountryTextField, _defaultCountryPickerView, "Country");

            set.Apply();

            ConsentPhoneSwitch.On = EditProspectViewModel.Prospect.FollowUpSettings.ConsentToPhone;

            FirstNameTextField.ShouldReturn = shouldReturn;
            MiddleNameTextField.ShouldReturn = shouldReturn;
            LastNameTextField.ShouldReturn = shouldReturn;
            AliasTextField.ShouldReturn = shouldReturn;
            EmailTextField.ShouldReturn = shouldReturn;
            MobilePhoneTextField.ShouldReturn = shouldReturn;
            WorkPhoneTextField.ShouldReturn = shouldReturn;
            WorkExtentionTextField.ShouldReturn = shouldReturn;
            HomePhoneTextField.ShouldReturn = shouldReturn;

            AddressLine1TextField.ShouldReturn = shouldReturn;
            AddressLine2TextField.ShouldReturn = shouldReturn;
            CityTextField.ShouldReturn = shouldReturn;
            CountyTextField.ShouldReturn = shouldReturn;
            ZipTextField.ShouldReturn = shouldReturn;

            CountryTextField.EditingDidBegin += (sender, e) =>
            {
                EditProspectViewModel.OriginalPickerValue = EditProspectViewModel.ActiveCountry;
                var row = EditProspectViewModel.Countries.IndexOf(EditProspectViewModel.OriginalPickerValue);
                _defaultCountryPickerView.Select(row, 0, true);
            };

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                EditProspectViewModel.CloseCommand.Execute(null);
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
                EditProspectViewModel.SaveCommand.Execute(null);

            });
            saveButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
            this.NavigationItem.SetRightBarButtonItem(saveButton, true);
            this.NavigationItem.Title = "Edit Prospect";
        }

        private void CustomizeTextFieldWithPicker(UITextField textField, UIPickerView pickerView, string pickerType)
        {
            textField.TintColor = UIColor.Clear;

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
            var toolbar = new UIToolbar();
            toolbar.SizeToFit();
            toolbar.AutoresizingMask = UIViewAutoresizing.All;
            var cancel = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered, (sender, e) =>
            {
                var row = 0;
                switch (pickerType)
                {
                    case "Country":
                        {
                            row = EditProspectViewModel.Countries.IndexOf(EditProspectViewModel.OriginalPickerValue);
                            EditProspectViewModel.ActiveCountry = EditProspectViewModel.OriginalPickerValue;
                            break;
                        };
                }

                pickerView.Select(row, 0, true);
                {
                    textField.ResignFirstResponder();
                }
            })
            {
                TintColor = ProspectManagementColors.AccentColor
            };

            var done = new UIBarButtonItem("Done", UIBarButtonItemStyle.Bordered, (sender, e) =>
            {
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
            set.Bind(CountryTextField).To(vm => vm.ActiveCountry.Description1).OneWay();
            _defaultCountryPickerView = new UIPickerView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ShowSelectionIndicator = true
            };
            _defaultCountryPickerViewModel = new CustomPickerView(_defaultCountryPickerView);
            _defaultCountryPickerView.Model = _defaultCountryPickerViewModel;
            set.Bind(_defaultCountryPickerViewModel).For(p => p.ItemsSource).To(vm => vm.Countries).OneWay();
            set.Bind(_defaultCountryPickerViewModel).For(p => p.SelectedItem).To(vm => vm.ActiveCountry).OneWayToSource();
        }

        private void CreateAlertViewBindings(MvxFluentBindingDescriptionSet<EditProspectView, EditProspectViewModel> set)
        {
            prefixPopup = new TextFieldWithPopup(PrefixTextField, this);
            prefixPopup.CustomAlertController = new CustomAlertController("Prefix");
            set.Bind(prefixPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.Prefixes);
            set.Bind(prefixPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActivePrefix);

            suffixPopup = new TextFieldWithPopup(SuffixTextField, this);
            suffixPopup.CustomAlertController = new CustomAlertController("Suffix");
            set.Bind(suffixPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.Suffixes);
            set.Bind(suffixPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveSuffix);

            statePopup = new TextFieldWithPopup(StateTextField, this);
            statePopup.CustomAlertController = new CustomAlertController("State");
            set.Bind(statePopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.States);
            set.Bind(statePopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveState);

            preferencePopup = new TextFieldWithPopup(ContactPreferenceTextField, this);
            preferencePopup.CustomAlertController = new CustomAlertController("Contact Preference");
            set.Bind(preferencePopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.ContactPreferences);
            set.Bind(preferencePopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveContactPreference);

            excludePopup = new TextFieldWithPopup(ExcludeReasonTextField, this);
            excludePopup.CustomAlertController = new CustomAlertController("Exclude Reason");
            set.Bind(excludePopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.ExcludeReasons);
            set.Bind(excludePopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveExcludeReason);

            trafficSourcePopup = new TextFieldWithPopup(TrafficSourceTextField, this);
            trafficSourcePopup.CustomAlertController = new CustomAlertController("Source");
            set.Bind(trafficSourcePopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.TrafficSources);
            set.Bind(trafficSourcePopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveTrafficSource);

            trafficDetailPopup = new TextFieldWithPopup(TrafficDetailTextField, this);
            trafficDetailPopup.CustomAlertController = new CustomAlertController("Detail");
            set.Bind(trafficDetailPopup.CustomAlertController).For(p => p.AlertController).To(vm => vm.ActiveTrafficSource.TrafficSourceDetails);
            set.Bind(trafficDetailPopup.CustomAlertController).For(p => p.SelectedCode).To(vm => vm.ActiveTrafficSourceDetail);

        }

    }
}