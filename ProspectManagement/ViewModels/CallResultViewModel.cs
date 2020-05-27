using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using MvvmValidation;
using ProspectManagement.Core.Extensions;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class CallResultViewModel : BaseViewModel, IMvxViewModel<PhoneCallActivity>
    {

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private ObservableCollection<UserDefinedCode> _callResults;
        private PhoneCallActivity _phoneCallActivity;
        private UserDefinedCode _currentCallResult;
        private IMvxCommand _saveCommand;
        private string _callResultError;
        private readonly IDialogService _dialogService;
        private readonly IActivityService _activityService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly IMvxNavigationService _navigationService;

        public PhoneCallActivity PhoneCallActivity
        {
            get { return _phoneCallActivity; }
            set
            {
                _phoneCallActivity = value;
                RaisePropertyChanged(() => PhoneCallActivity);
            }
        }

        public ObservableCollection<UserDefinedCode> CallResults
        {
            get { return _callResults; }
            set
            {
                _callResults = value;
                RaisePropertyChanged(() => CallResults);
            }
        }

        public UserDefinedCode CurrentCallResult
        {
            get { return _currentCallResult; }
            set
            {
                _currentCallResult = value;
                PhoneCallActivity.CallResult = _currentCallResult?.Code;
                RaisePropertyChanged(() => CurrentCallResult);
            }
        }

        public IMvxCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new MvxCommand(async () =>
                {
                    await ValidateAsync();
                    if (IsValid.GetValueOrDefault())
                    {
                        if (String.IsNullOrEmpty(PhoneCallActivity.ActivityId))
                        {
                            var resultUpdated = await _activityService.AddAdHocPhoneCallActivityAsync(PhoneCallActivity.ProspectAddressBook, PhoneCallActivity);
                            if (resultUpdated != null)
                            {
                                await _navigationService.Close(this);
                            }
                        }
                        else
                        {
                            var resultUpdated = await _activityService.UpdatePhoneCallActivityForProspectAsync(PhoneCallActivity);
                            if (resultUpdated)
                            {
                                await _navigationService.Close(this);
                            }
                        }

                    }
                    _hideAlertInteraction.Raise();
                }));
            }
        }

        public string CallResultError
        {
            get { return _callResultError; }
            set
            {
                _callResultError = value;
                RaisePropertyChanged(() => CallResultError);
            }
        }
        public CallResultViewModel(IUserDefinedCodeService userDefinedCodeService, IDialogService dialogService, IActivityService activityService, IMvxNavigationService navigationService)
        {
            _dialogService = dialogService;
            _activityService = activityService;
            _userDefinedCodeService = userDefinedCodeService;
            _navigationService = navigationService;

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        public void Prepare(PhoneCallActivity phoneCallActivity)
        {
            PhoneCallActivity = phoneCallActivity;
        }

        public override async Task Initialize()
        {
            CallResults = (await _userDefinedCodeService.GetCallResultUserDefinedCodes()).ToObservableCollection();
        }

        protected async void Validate()
        {
            await ValidateAsync();
        }

        protected async Task ValidateAsync()
        {
            var result = await Validator.ValidateAllAsync();

            UpdateValidationSummaryAndDetails(result);
        }

        private void UpdateValidationSummaryAndDetails(ValidationResult validationResult)
        {
            UpdateValidationSummary(validationResult);
            CallResultError = Validator.GetResult(nameof(CurrentCallResult)).ToString();
        }

        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (!IsValid.GetValueOrDefault(true))
            {
                ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummaryAndDetails(validationResult);
            }
        }

        private void ConfigureValidationRules()
        {
            Validator.AddRule(() => CurrentCallResult,
                    () =>
                    {
                        var result = !(CurrentCallResult == null || String.IsNullOrEmpty(CurrentCallResult.Code));
                        return RuleResult.Assert(result, string.Format("Call Result is required"));
                    });
        }

    }
}
