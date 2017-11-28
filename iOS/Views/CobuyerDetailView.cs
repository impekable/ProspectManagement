using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using ProspectManagement.Core.ViewModels;
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
    public partial class CobuyerDetailView : BaseView
    {
        AlertOverlay alertOverlay;

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

        private static readonly int TextFieldMargin = 10;
        public static readonly int ArrowWidth = 14;
        protected CobuyerDetailViewModel CobuyerDetailViewModel => ViewModel as CobuyerDetailViewModel;

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

        public CobuyerDetailView(IntPtr handle) : base(handle)
        {
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

            var set = this.CreateBindingSet<CobuyerDetailView, CobuyerDetailViewModel>();

            CreatePickerViewBindings(set);

            set.Bind(PrefixTextField).To(vm => vm.ActivePrefix.Description1);
            set.Bind(FirstNameTextField).To(vm => vm.FirstName).WithConversion(new TitleCaseValueConverter());
            set.Bind(MiddleNameTextField).To(vm => vm.MiddleName).WithConversion(new TitleCaseValueConverter());
            set.Bind(LastNameTextField).To(vm => vm.LastName).WithConversion(new TitleCaseValueConverter());
            set.Bind(SuffixTextField).To(vm => vm.ActiveSuffix.Description1);
            set.Bind(AliasTextField).To(vm => vm.NickName).WithConversion(new TitleCaseValueConverter());
            set.Bind(MobilePhoneTextField).To(vm => vm.MobilePhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(WorkPhoneTextField).To(vm => vm.WorkPhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(HomePhoneTextField).To(vm => vm.HomePhoneNumber).WithConversion(new PhoneNumberValueConverter());
            set.Bind(WorkExtensionTextField).To(vm => vm.WorkPhone.PhoneExtension);
            set.Bind(EmailTextField).To(vm => vm.Email.EmailAddress);

            set.Bind(AddressLine1TextField).To(vm => vm.StreetAddress.AddressLine1);
            set.Bind(AddressLine2TextField).To(vm => vm.StreetAddress.AddressLine2);
            set.Bind(CityTextField).To(vm => vm.StreetAddress.City);
            set.Bind(StateTextField).To(vm => vm.ActiveState.Description1);
            set.Bind(CountyTextField).To(vm => vm.StreetAddress.County);
            set.Bind(ZipCodeTextField).To(vm => vm.StreetAddress.PostalCode);
            set.Bind(CountryTextField).To(vm => vm.ActiveCountry.Description1);

            set.Bind(CountyLabel).To(vm => vm.ActiveCountry).WithConversion(new CountyConverter());
            set.Bind(StateCountryLabel).To(vm => vm.ActiveCountry).WithConversion(new StateCountryConverter());
            set.Bind(AddressSameAsBuyerSwitch).To(vm => vm.AddressSameAsBuyer);
            set.Bind(AddressLine1TextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(AddressLine2TextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(CityTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(StateTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(CountryTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(ZipCodeTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(CountyTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);
            set.Bind(HomePhoneTextField).For(v => v.Enabled).To(vm => vm.EnableAddressFields);

            set.Bind(FirstNameErrorLabel).To(vm => vm.FirstNameError);
            set.Bind(LastNameErrorLabel).To(vm => vm.LastNameError);
            set.Bind(HomePhoneErrorLabel).To(vm => vm.HomePhoneNumberError);
            set.Bind(WorkPhoneErrorLabel).To(vm => vm.WorkPhoneNumberError);
            set.Bind(MobilePhoneErrorLabel).To(vm => vm.MobilePhoneNumberError);
            set.Bind(EmailErrorLabel).To(vm => vm.EmailAddressError);
            set.Bind(ValidationErrorLabel).To(vm => vm.ValidationErrorsString);

            set.Bind(StateTextField).For(v => v.Hidden).To(vm => vm.ForeignState);

            set.Bind(this).For(view => view.HideAlertInteraction).To(viewModel => viewModel.HideAlertInteraction).OneWay();

            CustomizeTextField(PrefixTextField, _defaultPrefixPickerView, "Prefix");
            CustomizeTextField(SuffixTextField, _defaultSuffixPickerView, "Suffix");
            CustomizeTextField(StateTextField, _defaultStatePickerView, "State");
            CustomizeTextField(CountryTextField, _defaultCountryPickerView, "Country");

            if (CobuyerDetailViewModel.Cobuyer != null)
            {
                AddressSameAsBuyerSwitch.On = CobuyerDetailViewModel.Cobuyer.AddressSameAsBuyer;
            }

            set.Apply();

            PrefixTextField.ShouldReturn = shouldReturn;
            FirstNameTextField.ShouldReturn = shouldReturn;
            MiddleNameTextField.ShouldReturn = shouldReturn;
            LastNameTextField.ShouldReturn = shouldReturn;
            SuffixTextField.ShouldReturn = shouldReturn;
            AliasTextField.ShouldReturn = shouldReturn;
            EmailTextField.ShouldReturn = shouldReturn;
            MobilePhoneTextField.ShouldReturn = shouldReturn;
            WorkPhoneTextField.ShouldReturn = shouldReturn;
            WorkExtensionTextField.ShouldReturn = shouldReturn;
            HomePhoneTextField.ShouldReturn = shouldReturn;

            AddressLine1TextField.ShouldReturn = shouldReturn;
            AddressLine2TextField.ShouldReturn = shouldReturn;
            CityTextField.ShouldReturn = shouldReturn;
            StateTextField.ShouldReturn = shouldReturn;
            CountryTextField.ShouldReturn = shouldReturn;
            CountyTextField.ShouldReturn = shouldReturn;
            ZipCodeTextField.ShouldReturn = shouldReturn;

            PrefixTextField.EditingDidBegin += (sender, e) =>
            {
                CobuyerDetailViewModel.OriginalPickerValue = CobuyerDetailViewModel.ActivePrefix;
                if (CobuyerDetailViewModel.ActivePrefix == null)
                    CobuyerDetailViewModel.ActivePrefix = CobuyerDetailViewModel.Prefixes != null ? CobuyerDetailViewModel.Prefixes.FirstOrDefault() : null;
            };
            SuffixTextField.EditingDidBegin += (sender, e) =>
            {
                CobuyerDetailViewModel.OriginalPickerValue = CobuyerDetailViewModel.ActiveSuffix;
                if (CobuyerDetailViewModel.ActiveSuffix == null)
                    CobuyerDetailViewModel.ActiveSuffix = CobuyerDetailViewModel.Suffixes.FirstOrDefault();
            };
            StateTextField.EditingDidBegin += (sender, e) =>
            {
                CobuyerDetailViewModel.OriginalPickerValue = CobuyerDetailViewModel.ActiveState;
                if (CobuyerDetailViewModel.ActiveState == null)
                    CobuyerDetailViewModel.ActiveState = CobuyerDetailViewModel.States.FirstOrDefault();
            };
            CountryTextField.EditingDidBegin += (sender, e) =>
            {
                CobuyerDetailViewModel.OriginalPickerValue = CobuyerDetailViewModel.ActiveCountry;
                if (CobuyerDetailViewModel.ActiveCountry == null)
                    CobuyerDetailViewModel.ActiveCountry = CobuyerDetailViewModel.Countries.FirstOrDefault();
            };

            var cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                CobuyerDetailViewModel.CloseCommand.Execute(null);
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

                CobuyerDetailViewModel.SaveCommand.Execute(null);

            });
            saveButton.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("Raleway-Bold", 18),
                TextColor = UIColor.White
            }, UIControlState.Normal);

            this.NavigationItem.SetLeftBarButtonItem(cancelButton, true);
            this.NavigationItem.SetRightBarButtonItem(saveButton, true);


            if (CobuyerDetailViewModel.Cobuyer.CobuyerAddressNumber > 0)
            {
                this.NavigationItem.Title = "Edit Cobuyer";
            }
            else
            {

                this.NavigationItem.Title = "Add Cobuyer";
            }

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
            var toolbar = new UIToolbar(new CGRect(0, 0, 320, 44));
            var cancel = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered, (sender, e) =>
            {
                var row = 0;
                switch (pickerType)
                {
                    case "State":
                        {
                            row = CobuyerDetailViewModel.States.IndexOf(CobuyerDetailViewModel.OriginalPickerValue);
                            CobuyerDetailViewModel.ActiveState = null;
                            break;
                        };
                    case "Country":
                        {
                            row = CobuyerDetailViewModel.Countries.IndexOf(CobuyerDetailViewModel.OriginalPickerValue);
                            CobuyerDetailViewModel.ActiveCountry = null;
                            break;
                        };
                    case "Suffix":
                        {
                            row = CobuyerDetailViewModel.Suffixes.IndexOf(CobuyerDetailViewModel.OriginalPickerValue);
                            CobuyerDetailViewModel.ActiveSuffix = null;
                            break;
                        };
                    case "Prefix":
                        {
                            row = CobuyerDetailViewModel.Prefixes.IndexOf(CobuyerDetailViewModel.OriginalPickerValue);
                            CobuyerDetailViewModel.ActivePrefix = null;
                            break;
                        };

                    default: { break; };
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

        private void CreatePickerViewBindings(MvxFluentBindingDescriptionSet<CobuyerDetailView, CobuyerDetailViewModel> set)
        {
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

        }
    }
}