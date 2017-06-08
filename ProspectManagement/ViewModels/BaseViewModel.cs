using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmValidation;
using System.ComponentModel;
using System.Collections;

namespace ProspectManagement.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel, IValidatable, INotifyDataErrorInfo, IDisposable
    {
		protected ValidationHelper Validator { get; }

		private bool? _isValid;
		private string _validationErrorsString;
		private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; }

		public bool HasErrors => NotifyDataErrorInfoAdapter.HasErrors;

		public string ValidationErrorsString
		{
			get { return _validationErrorsString; }
			private set
			{
				_validationErrorsString = value;
				RaisePropertyChanged(nameof(ValidationErrorsString));
			}
		}

		protected bool? IsValid
		{
			get { return _isValid; }
			private set
			{
				_isValid = value;
				RaisePropertyChanged(nameof(IsValid));
			}
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
		{
			add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
			remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
		}

		public BaseViewModel()
		{

			Validator = new ValidationHelper();

			NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);
			NotifyDataErrorInfoAdapter.ErrorsChanged += OnErrorsChanged;

		}

		private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
		{
			// Notify the UI that the property has changed so that the validation error gets displayed (or removed).
			RaisePropertyChanged(e.PropertyName);
		}

		Task<ValidationResult> IValidatable.Validate()
		{
			return Validator.ValidateAllAsync();
		}

		protected void UpdateValidationSummary(ValidationResult validationResult)
		{
			IsValid = validationResult.IsValid;
			ValidationErrorsString = validationResult.ToString();
		}

		protected async Task ReloadDataAsync()
        {
            try
            {
                await InitializeAsync();
            }
            catch (Exception ex )
            {

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        protected virtual Task InitializeAsync()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }
        		
    }
}
