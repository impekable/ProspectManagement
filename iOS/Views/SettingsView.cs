using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using MvvmCross.Binding.iOS.Views;
using ProspectManagement.Core.ViewModels;
using CoreGraphics;
using ProspectManagement.iOS.Utility;
using MvvmCross.Binding.BindingContext;
using ProspectManagement.Core.Converters;
using MvvmCross.iOS.Views.Presenters.Attributes;

namespace ProspectManagement.iOS.Views
{
	public partial class SettingsView : BaseView
	{
		private UIPickerView _defaultCommunityPickerView;
		private MvxPickerViewModel _defaultCommunityPickerViewModel;
		private static readonly int TextFieldMargin = 10;
		public static readonly int ArrowWidth = 14;
		protected SettingsViewModel SettingsViewModel => ViewModel as SettingsViewModel;
		public SettingsView(IntPtr handle) : base(handle)
		{
		}

		protected override void CreateBindings()
		{
			base.CreateBindings();
			var set = this.CreateBindingSet<SettingsView, SettingsViewModel>();
			//CreateCommunityBindings(set);

			//set.Bind(DataActivityIndicator).For(v => v.IsAnimating).To(vm => vm.IsActivityIndicatorActive);
			//set.Bind(DataActivityIndicator).For(v => v.Hidden).To(vm => vm.IsActivityIndicatorActive).WithConversion(new InverseValueConverter());
			//set.Bind(CommunityTextField).For(v => v.Enabled).To(vm => vm.IsUserLoggedIn);
			//set.Bind(LoggedInAsLabel).To(vm => vm.ActiveUser).WithConversion(new LoggedInAsConverter());
			//set.Bind(LoginButton).For(v => v.Hidden).To(vm => vm.IsUserLoggedIn);
			//set.Bind(ButtonSave).For(v => v.Hidden).To(vm => vm.IsUserLoggedIn).WithConversion(new InverseValueConverter());
			set.Bind(ButtonSave).To(vm => vm.SwitchCommunityCommand);

			//CustomizeTextField(CommunityTextField, _defaultCommunityPickerView);
			set.Apply();
		}

		private void CustomizeTextField(UITextField textField, UIPickerView pickerView)
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

			var toolbar = CreateToolbar();
			textField.InputAccessoryView = toolbar;
			textField.Layer.BorderColor = ProspectManagementColors.BorderColor.CGColor;
			textField.Layer.BorderWidth = 1;
		}

		private UIToolbar CreateToolbar()
		{
			var toolbar = new UIToolbar(new CGRect(0, 0, 320, 44));
			var done = new UIBarButtonItem("OK", UIBarButtonItemStyle.Bordered, (sender, e) =>
			{
				//CommunityTextField.ResignFirstResponder();
			})
			{
				TintColor = ProspectManagementColors.AccentColor
			};
			toolbar.SetItems(new[] { done }, false);
			return toolbar;
		}

		private void CreateCommunityBindings(MvxFluentBindingDescriptionSet<SettingsView, SettingsViewModel> set)
		{
			//set.Bind(CommunityTextField).To(vm => vm.DefaultCommunity.Description).OneWay();
			_defaultCommunityPickerView = new UIPickerView
			{
				ShowSelectionIndicator = true
			};

			_defaultCommunityPickerViewModel = new MvxPickerViewModel(_defaultCommunityPickerView);
			_defaultCommunityPickerView.Model = _defaultCommunityPickerViewModel;
			set.Bind(_defaultCommunityPickerViewModel).For(p => p.ItemsSource).To(vm => vm.Communities).OneWay();
			set.Bind(_defaultCommunityPickerViewModel).For(p => p.SelectedItem).To(vm => vm.DefaultCommunity).OneWayToSource();
		}
	}
}