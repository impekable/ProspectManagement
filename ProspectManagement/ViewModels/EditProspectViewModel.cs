using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmValidation;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Extensions;
using System.Threading.Tasks;
using System.Linq;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Messages;

namespace ProspectManagement.Core.ViewModels
{
    public class EditProspectViewModel : BaseViewModel
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private UserDefinedCode _originalPickerValue;
        private TrafficSource _originalTrafficSource;
        private TrafficSourceDetail _originalTrafficSourceDetail;

        private Prospect _prospect;
        private Prospect _originalProspect;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _nickName;
        private StreetAddress _streetAddress;
        private FollowUpSettings _followUpSettings;
        private PhoneNumber _mobilePhoneNumber;
        private PhoneNumber _workPhoneNumber;
        private PhoneNumber _homePhoneNumber;
        private Email _email;

        private readonly ITrafficSourceService _trafficSourceService;
        private readonly IDialogService _dialogService;
        private readonly IEmailValidationService _emailValidationService;
        private readonly IPhoneNumberValidationService _phoneNumberValidationService;
        private readonly IStreetValidationService _streetValidationService;
        private readonly IProspectService _prospectService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly IProspectCache _prospectCache;
        protected IMvxMessenger Messenger;

        private ICommand _saveCommand;
        private ICommand _closeCommand;

        private bool _isSelectedTrafficSource;

        private ObservableCollection<UserDefinedCode> _prefixes;
        private ObservableCollection<UserDefinedCode> _suffixes;
        private ObservableCollection<UserDefinedCode> _states;
        private ObservableCollection<UserDefinedCode> _countries;
        private ObservableCollection<UserDefinedCode> _contactPreferences;
        private ObservableCollection<UserDefinedCode> _excludeReasons;
        private ObservableCollection<TrafficSource> _trafficSources;

        private UserDefinedCode _activePrefix;
        private UserDefinedCode _activeSuffix;
        private UserDefinedCode _activeState;
        private UserDefinedCode _activeCountry;
        private UserDefinedCode _activeContactPreference;
        private UserDefinedCode _activeExcludeReason;
        private TrafficSource _activeTrafficSource;
        private TrafficSourceDetail _activeTrafficSourceDetail;

        private string _firstNameError;
        private string _lastNameError;
        private string _emailAddressError;
        private string _mobilePhoneNumberError;
        private string _workPhoneNumberError;
        private string _homePhoneNumberError;
        private string _trafficSourceDetailError;

        public TrafficSource OriginalTrafficSource
        {
            get { return _originalTrafficSource; }
            set
            {
                _originalTrafficSource = value;
                RaisePropertyChanged(() => OriginalTrafficSource);
            }
        }

        public TrafficSourceDetail OriginalTrafficSourceDetail
        {
            get { return _originalTrafficSourceDetail; }
            set
            {
                _originalTrafficSourceDetail = value;
                RaisePropertyChanged(() => OriginalTrafficSourceDetail);
            }
        }

        public UserDefinedCode OriginalPickerValue
        {
            get { return _originalPickerValue; }
            set
            {
                _originalPickerValue = value;
                RaisePropertyChanged(() => OriginalPickerValue);
            }
        }

        public UserDefinedCode ActiveExcludeReason
        {
            get { return _activeExcludeReason; }
            set
            {
                _activeExcludeReason = value;
                FollowUpSettings.ExcludeReason = _activeExcludeReason != null ? _activeExcludeReason.Code : String.Empty;
                RaisePropertyChanged(() => ActiveExcludeReason);
            }
        }

        public UserDefinedCode ActiveContactPreference
        {
            get { return _activeContactPreference; }
            set
            {
                _activeContactPreference = value;
                FollowUpSettings.PreferredContactMethod = _activeContactPreference != null ? _activeContactPreference.Description1 : String.Empty;
                RaisePropertyChanged(() => ActiveContactPreference);
            }
        }

        public UserDefinedCode ActivePrefix
        {
            get { return _activePrefix; }
            set
            {
                _activePrefix = value == null || String.IsNullOrEmpty(value.Code) ? null : value;
                RaisePropertyChanged(() => ActivePrefix);
            }
        }

        public UserDefinedCode ActiveSuffix
        {
            get { return _activeSuffix; }
            set
            {
                _activeSuffix = value == null || String.IsNullOrEmpty(value.Code) ? null : value;
                RaisePropertyChanged(() => ActiveSuffix);
            }
        }

        public UserDefinedCode ActiveState
        {
            get { return _activeState; }
            set
            {
                _activeState = value == null || String.IsNullOrEmpty(value.Code) ? null : value;
                StreetAddress.State = _activeState == null ? String.Empty : _activeState.Code;
                RaisePropertyChanged(() => ActiveState);
            }
        }

        public UserDefinedCode ActiveCountry
        {
            get { return _activeCountry; }
            set
            {
                _activeCountry = value == null || String.IsNullOrEmpty(value.Code) ? null : value;
                StreetAddress.Country = _activeCountry == null ? String.Empty : _activeCountry.Code;
                RaisePropertyChanged(() => ActiveCountry);
                RaisePropertyChanged(() => ForeignState);
            }
        }

        public ObservableCollection<UserDefinedCode> ExcludeReasons
        {
            get { return _excludeReasons; }
            set
            {
                _excludeReasons = value;
                RaisePropertyChanged(() => ExcludeReasons);
            }
        }

        public ObservableCollection<UserDefinedCode> ContactPreferences
        {
            get { return _contactPreferences; }
            set
            {
                _contactPreferences = value;
                RaisePropertyChanged(() => ContactPreferences);
            }
        }

        public ObservableCollection<UserDefinedCode> Prefixes
        {
            get { return _prefixes; }
            set
            {
                _prefixes = value;
                RaisePropertyChanged(() => Prefixes);
            }
        }

        public ObservableCollection<UserDefinedCode> Suffixes
        {
            get { return _suffixes; }
            set
            {
                _suffixes = value;
                RaisePropertyChanged(() => Suffixes);
            }
        }

        public ObservableCollection<UserDefinedCode> States
        {
            get { return _states; }
            set
            {
                _states = value;
                RaisePropertyChanged(() => States);
            }
        }

        public ObservableCollection<UserDefinedCode> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                RaisePropertyChanged(() => Countries);
            }
        }

        public string FirstNameError
        {
            get { return _firstNameError; }
            set
            {
                _firstNameError = value;
                RaisePropertyChanged(() => FirstNameError);
            }
        }

        public string LastNameError
        {
            get { return _lastNameError; }
            set
            {
                _lastNameError = value;
                RaisePropertyChanged(() => LastNameError);
            }
        }

        public string EmailAddressError
        {
            get { return _emailAddressError; }
            set
            {
                _emailAddressError = value;
                RaisePropertyChanged(() => EmailAddressError);
            }
        }

        public string MobilePhoneNumberError
        {
            get { return _mobilePhoneNumberError; }
            set
            {
                _mobilePhoneNumberError = value;
                RaisePropertyChanged(() => MobilePhoneNumberError);
            }
        }

        public string WorkPhoneNumberError
        {
            get { return _workPhoneNumberError; }
            set
            {
                _workPhoneNumberError = value;
                RaisePropertyChanged(() => WorkPhoneNumberError);
            }
        }

        public string HomePhoneNumberError
        {
            get { return _homePhoneNumberError; }
            set
            {
                _homePhoneNumberError = value;
                RaisePropertyChanged(() => HomePhoneNumberError);
            }
        }

        public string TrafficSourceDetailError
        {
            get { return _trafficSourceDetailError; }
            set
            {
                _trafficSourceDetailError = value;
                RaisePropertyChanged(() => TrafficSourceDetailError);
            }
        }

        public bool IsSelectedTrafficSource
        {
            get { return _isSelectedTrafficSource; }
            set
            {
                _isSelectedTrafficSource = value;
                RaisePropertyChanged(() => IsSelectedTrafficSource);
            }
        }

        public TrafficSourceDetail ActiveTrafficSourceDetail
        {
            get { return _activeTrafficSourceDetail; }
            set
            {
                _activeTrafficSourceDetail = value;
                RaisePropertyChanged(() => ActiveTrafficSourceDetail);
                Validator.ValidateAsync(nameof(ActiveTrafficSourceDetail));
            }
        }

        public TrafficSource ActiveTrafficSource
        {
            get { return _activeTrafficSource; }
            set
            {
                if (_activeTrafficSource != value)
                {
                    ActiveTrafficSourceDetail = null;
                }
                _activeTrafficSource = value;
                IsSelectedTrafficSource = (_activeTrafficSource != null);
                RaisePropertyChanged(() => ActiveTrafficSource);
            }
        }

        public ObservableCollection<TrafficSource> TrafficSources
        {
            get { return _trafficSources; }
            set
            {
                _trafficSources = value;
                RaisePropertyChanged(() => TrafficSources);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new MvxCommand(async () =>
                {
                    var prospectUpdated = false;
                    await ValidateAsync();
                    if (IsValid.GetValueOrDefault())
                    {
                        Prospect.Name = LastName + ", " + FirstName;
                        Prospect.FirstName = FirstName;
                        Prospect.LastName = LastName;
                        Prospect.MiddleName = MiddleName;
                        Prospect.NickName = NickName;
                        Prospect.NamePrefix = _activePrefix != null ? _activePrefix.Description1 : String.Empty;
                        Prospect.NameSuffix = _activeSuffix != null ? _activeSuffix.Description1 : String.Empty;
                        Prospect.StreetAddress = !String.IsNullOrEmpty(StreetAddress.AddressLine1) ||
                                                       !String.IsNullOrEmpty(StreetAddress.AddressLine2) ||
                                                       !String.IsNullOrEmpty(StreetAddress.City) ||
                                                       !String.IsNullOrEmpty(StreetAddress.State) ||
                                                        !String.IsNullOrEmpty(StreetAddress.PostalCode) ? StreetAddress : null;
                        Prospect.FollowUpSettings = FollowUpSettings;
                        Prospect.MobilePhoneNumber = MobilePhone;
                        Prospect.WorkPhoneNumber = WorkPhone;
                        Prospect.HomePhoneNumber = HomePhone;
                        Prospect.Email = Email;
                        Prospect.TrafficSourceCodeId = ActiveTrafficSourceDetail != null ? _activeTrafficSourceDetail.CodeId : 0;

                        prospectUpdated = await _prospectService.UpdateProspectAsync(Prospect);
                    }
                    _hideAlertInteraction.Raise();

                    if (prospectUpdated)
                    {
                        Messenger.Publish(new ProspectChangedMessage(this) { UpdatedProspect = Prospect });

                        Close(this);
                    }
                    else
                    {
                        Prospect.Name = _originalProspect.Name;
                        Prospect.FirstName = _originalProspect.FirstName;
                        Prospect.LastName = _originalProspect.LastName;
                        Prospect.MiddleName = _originalProspect.MiddleName;
                        Prospect.NickName = _originalProspect.NickName;
                        Prospect.NamePrefix = _originalProspect.NamePrefix;
                        Prospect.NameSuffix = _originalProspect.NameSuffix;
                        Prospect.TrafficSourceCodeId = _originalProspect.TrafficSourceCodeId;
                        Prospect.FollowUpSettings = _originalProspect.FollowUpSettings.ShallowCopy();
                        Prospect.StreetAddress = _originalProspect.StreetAddress.ShallowCopy();
                        Prospect.MobilePhoneNumber = _originalProspect.MobilePhoneNumber.ShallowCopy();
                        Prospect.HomePhoneNumber = _originalProspect.HomePhoneNumber.ShallowCopy();
                        Prospect.WorkPhoneNumber = _originalProspect.WorkPhoneNumber.ShallowCopy();
                        Prospect.Email = _originalProspect.Email.ShallowCopy();
                    }
                }));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(() => { Close(this); }));
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => FromLead);
                RaisePropertyChanged(() => Prospect);
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(() => FirstName);
                Validator.ValidateAsync(nameof(FirstName));
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged(() => LastName);
                Validator.ValidateAsync(nameof(LastName));
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                RaisePropertyChanged(() => MiddleName);
                Validator.ValidateAsync(nameof(MiddleName));
            }
        }

        public string NickName
        {
            get { return _nickName; }
            set
            {
                _nickName = value;
                RaisePropertyChanged(() => NickName);
                Validator.ValidateAsync(nameof(NickName));
            }
        }

        public StreetAddress StreetAddress
        {
            get { return _streetAddress; }
            set
            {
                _streetAddress = value;
                RaisePropertyChanged(() => StreetAddress);
            }
        }

        public bool ForeignState
        {
            get { return !(ActiveCountry == null || String.IsNullOrEmpty(ActiveCountry.Code) || ActiveCountry.Code.Equals("US")); }
        }

        public bool FromLead
        {
            get { return Prospect.ProspectCommunity.LeadId > 0; }
        }

        public FollowUpSettings FollowUpSettings
        {
            get { return _followUpSettings; }
            set
            {
                _followUpSettings = value;
                RaisePropertyChanged(() => FollowUpSettings);
                Validator.ValidateAsync(nameof(FollowUpSettings));
            }
        }

        public bool ExcludeFromFollowup
        {
            get { return _followUpSettings.ExcludeFromFollowup; }
            set
            {
                _followUpSettings.ExcludeFromFollowup = value;
                if (!_followUpSettings.ExcludeFromFollowup)
                    ActiveExcludeReason = null;
                RaisePropertyChanged(() => ExcludeFromFollowup);
                Validator.ValidateAsync(nameof(ExcludeFromFollowup));
            }
        }

        public PhoneNumber MobilePhone
        {
            get { return _mobilePhoneNumber; }
            set
            {
                _mobilePhoneNumber = value;
                RaisePropertyChanged(() => MobilePhone);
            }
        }

        public String MobilePhoneNumber
        {
            get { return _mobilePhoneNumber.Phone; }
            set
            {
                _mobilePhoneNumber.Phone = value;
                RaisePropertyChanged(() => MobilePhoneNumber);
            }
        }

        public PhoneNumber WorkPhone
        {
            get { return _workPhoneNumber; }
            set
            {
                _workPhoneNumber = value;
                RaisePropertyChanged(() => WorkPhone);
            }
        }

        public String WorkPhoneNumber
        {
            get { return _workPhoneNumber.Phone; }
            set
            {
                _workPhoneNumber.Phone = value;
                RaisePropertyChanged(() => WorkPhoneNumber);
            }
        }

        public PhoneNumber HomePhone
        {
            get { return _homePhoneNumber; }
            set
            {
                _homePhoneNumber = value;
                RaisePropertyChanged(() => HomePhone);
            }
        }

        public String HomePhoneNumber
        {
            get { return _homePhoneNumber.Phone; }
            set
            {
                _homePhoneNumber.Phone = value;
                RaisePropertyChanged(() => HomePhoneNumber);
            }
        }

        public Email Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        public EditProspectViewModel(IMvxMessenger messenger, IUserDefinedCodeService userDefinedCodeService, IStreetValidationService streetValidationService, ITrafficSourceService trafficSourceService, IDialogService dialogService, IPhoneNumberValidationService phoneNumberValidationService, IEmailValidationService emailValidationService, IProspectCache prospectCache, IProspectService prospectService)
        {
            Messenger = messenger;
            _trafficSourceService = trafficSourceService;
            _dialogService = dialogService;
            _phoneNumberValidationService = phoneNumberValidationService;
            _streetValidationService = streetValidationService;
            _emailValidationService = emailValidationService;
            _prospectService = prospectService;
            _userDefinedCodeService = userDefinedCodeService;
            _prospectCache = prospectCache;

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;

        }

        public async void Init(Prospect prospect)
        {
            Prospect = _prospectCache.GetProspectFromCache(prospect.ProspectAddressNumber);
            if (Prospect == null)
            {
                Prospect = await _prospectService.GetProspectAsync(prospect.ProspectAddressNumber);
            }

            FirstName = Prospect.FirstName;
            LastName = Prospect.LastName;
            MiddleName = Prospect.MiddleName;
            NickName = Prospect.NickName;
            StreetAddress = Prospect.StreetAddress == null ? new StreetAddress() : Prospect.StreetAddress.ShallowCopy();
			FollowUpSettings = Prospect.FollowUpSettings.ShallowCopy();
            MobilePhone = Prospect.MobilePhoneNumber == null ? new PhoneNumber() : Prospect.MobilePhoneNumber.ShallowCopy();
			WorkPhone = Prospect.WorkPhoneNumber == null ? new PhoneNumber() : Prospect.WorkPhoneNumber.ShallowCopy();
			HomePhone = Prospect.HomePhoneNumber == null ? new PhoneNumber() : Prospect.HomePhoneNumber.ShallowCopy();
			Email = Prospect.Email == null ? new Email() : Prospect.Email.ShallowCopy();

            _originalProspect = Prospect.ShallowCopy();
            _originalProspect.FollowUpSettings = FollowUpSettings.ShallowCopy();
			_originalProspect.StreetAddress = StreetAddress.ShallowCopy();
            _originalProspect.MobilePhoneNumber = MobilePhone.ShallowCopy();
            _originalProspect.WorkPhoneNumber = WorkPhone.ShallowCopy();
            _originalProspect.HomePhoneNumber = HomePhone.ShallowCopy();
            _originalProspect.Email = Email.ShallowCopy();

            Prefixes = (await _userDefinedCodeService.GetPrefixUserDefinedCodes()).ToObservableCollection();
            ActivePrefix = Prefixes.FirstOrDefault(p => p.Description1 == Prospect.NamePrefix);

            Suffixes = (await _userDefinedCodeService.GetSuffixUserDefinedCodes()).ToObservableCollection();
            ActiveSuffix = Suffixes.FirstOrDefault(p => p.Description1 == Prospect.NameSuffix);

            ContactPreferences = (await _userDefinedCodeService.GetContactPreferenceUserDefinedCodes()).ToObservableCollection();
            ActiveContactPreference = ContactPreferences.FirstOrDefault(p => p.Description1 == Prospect.FollowUpSettings.PreferredContactMethod);

            ExcludeReasons = (await _userDefinedCodeService.GetExcludeReasonUserDefinedCodes()).ToObservableCollection();
            ActiveExcludeReason = ExcludeReasons.FirstOrDefault(p => p.Code == Prospect.FollowUpSettings.ExcludeReason);

            States = (await _userDefinedCodeService.GetStateUserDefinedCodes()).ToObservableCollection();
            ActiveState = Prospect.StreetAddress != null ? States.FirstOrDefault(p => p.Code == Prospect.StreetAddress.State) : null;

            Countries = (await _userDefinedCodeService.GetCountryUserDefinedCodes()).ToObservableCollection();
            ActiveCountry = Prospect.StreetAddress != null ? Countries.FirstOrDefault(p => p.Code == Prospect.StreetAddress.Country) : null;
           
            TrafficSources = (await _trafficSourceService.GetTrafficSourcesByDivision(Prospect.ProspectCommunity.Division)).ToObservableCollection();
            ActiveTrafficSource = TrafficSources.FirstOrDefault(t => t.TrafficSourceDetails.Any(td => td.CodeId == Prospect.TrafficSourceCodeId));
            if (ActiveTrafficSource != null)
                ActiveTrafficSourceDetail = ActiveTrafficSource.TrafficSourceDetails.First(td => td.CodeId == Prospect.TrafficSourceCodeId);


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
            FirstNameError = Validator.GetResult(nameof(FirstName)).ToString();
            LastNameError = Validator.GetResult(nameof(LastName)).ToString();
            EmailAddressError = Validator.GetResult(nameof(Email)).ToString();
            MobilePhoneNumberError = Validator.GetResult(nameof(MobilePhone)).ToString();
            HomePhoneNumberError = Validator.GetResult(nameof(HomePhone)).ToString();
            WorkPhoneNumberError = Validator.GetResult(nameof(WorkPhone)).ToString();
            TrafficSourceDetailError = Validator.GetResult(nameof(ActiveTrafficSourceDetail)).ToString();
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
            Validator.AddRule(nameof(FirstName),
                  () => RuleResult.Assert(!string.IsNullOrEmpty(FirstName) && FirstName.Length <= 25, "First Name is required and cannot be more than 25 characters"));

            Validator.AddRule(nameof(MiddleName),
                  () => RuleResult.Assert(string.IsNullOrEmpty(MiddleName) || MiddleName.Length <= 25, "Middle Name cannot be more than 25 characters"));

            Validator.AddRule(nameof(LastName),
                  () => RuleResult.Assert(!string.IsNullOrEmpty(LastName) && LastName.Length <= 25, "Last Name is required and cannot be more than 25 characters"));

            Validator.AddRule(nameof(NickName),
                  () => RuleResult.Assert(string.IsNullOrEmpty(NickName) || NickName.Length <= 40, "Alias cannot be more than 40 characters"));

            Validator.AddRequiredRule(() => FollowUpSettings.PreferredContactMethod, "Contact Preference is required");

            Validator.AddAsyncRule(() => Email,
                   async () =>
                    {
                        var verifyViaService = (!Email.EmailVerified || (Email.EmailAddress != _originalProspect.Email.EmailAddress))
                                                             && !String.IsNullOrEmpty(Email.EmailAddress);
                        var result = !verifyViaService
                                           ? new EmailValidationResult() { IsValid = true }
                                           : await _emailValidationService.Validate(Email);

                        return RuleResult.Assert(result.IsValid,
                                                        string.Format("Valid email required" + (result.DidYouMean != null ? ". Did you mean " + result.DidYouMean : "")));
                    });

            Validator.AddAsyncRule(() => MobilePhone,
                   async () =>
                    {
                        var verifyViaService = (!MobilePhone.PhoneVerified || (MobilePhone.Phone != _originalProspect.MobilePhoneNumber.Phone))
                                                            && !String.IsNullOrEmpty(MobilePhone.Phone);
                        var result = !verifyViaService ? true :
                                           await _phoneNumberValidationService.Validate(MobilePhone);

                        return RuleResult.Assert(result,
                            string.Format("Mobile Phone number is invalid"));
                    });

            Validator.AddAsyncRule(() => HomePhone,
                   async () =>
                    {
                        var verifyViaService = (!HomePhone.PhoneVerified || (HomePhone.Phone != _originalProspect.HomePhoneNumber.Phone))
                                                            && !String.IsNullOrEmpty(HomePhone.Phone);
                        var result = !verifyViaService ? true :
                                           await _phoneNumberValidationService.Validate(HomePhone);

                        return RuleResult.Assert(result,
                            string.Format("Home Phone number is invalid"));
                    });

            Validator.AddAsyncRule(() => WorkPhone,
                   async () =>
                    {
                        var verifyViaService = (!WorkPhone.PhoneVerified || (WorkPhone.Phone != _originalProspect.WorkPhoneNumber.Phone))
                                                            && !String.IsNullOrEmpty(WorkPhone.Phone);
                        var result = !verifyViaService ? true :
                                           await _phoneNumberValidationService.Validate(WorkPhone);

                        return RuleResult.Assert(result,
                            string.Format("Work Phone number is invalid"));
                    });

            Validator.AddAsyncRule(() => StreetAddress,
                   async () =>
                    {
                        var verifyViaService = (!StreetAddress.StreetAddressVerified ||
                                                (StreetAddress.AddressLine1 != _originalProspect.StreetAddress.AddressLine1) ||
                                                (StreetAddress.AddressLine2 != _originalProspect.StreetAddress.AddressLine2) ||
                                                (StreetAddress.City != _originalProspect.StreetAddress.City) ||
                                                (StreetAddress.State != _originalProspect.StreetAddress.State) ||
                                                (StreetAddress.PostalCode != _originalProspect.StreetAddress.PostalCode))
                                               && (!String.IsNullOrEmpty(StreetAddress.AddressLine1) ||
                                                       !String.IsNullOrEmpty(StreetAddress.AddressLine2) ||
                                                       !String.IsNullOrEmpty(StreetAddress.City) ||
                                                       !String.IsNullOrEmpty(StreetAddress.State) ||
                                                        !String.IsNullOrEmpty(StreetAddress.PostalCode));

                        var result = !verifyViaService
                                           ? true :
                                           await _streetValidationService.Validate(StreetAddress);

                        return RuleResult.Assert(result,
                            string.Format("Street Address is invalid"));
                    });

            Validator.AddRule(() => ActiveTrafficSourceDetail,
                    () =>
                    {
                        var result = !(Prospect.ProspectCommunity.LeadId == 0 && (ActiveTrafficSourceDetail == null || ActiveTrafficSourceDetail.CodeId == 0));

                        return RuleResult.Assert(result, string.Format("Traffic Detail required"));
                    });

            Validator.AddRule(() => Prospect,
                    () =>
                    {
                        var result = !(String.IsNullOrEmpty(Email.EmailAddress) &&
                                           String.IsNullOrEmpty(MobilePhone.Phone) &&
                                           String.IsNullOrEmpty(HomePhone.Phone) &&
                                           String.IsNullOrEmpty(WorkPhone.Phone) &&
                                           String.IsNullOrEmpty(StreetAddress.AddressLine1) &&
                                           String.IsNullOrEmpty(StreetAddress.AddressLine2) &&
                                           String.IsNullOrEmpty(StreetAddress.City) &&
                                           String.IsNullOrEmpty(StreetAddress.State) &&
                                       String.IsNullOrEmpty(StreetAddress.PostalCode));

                        return RuleResult.Assert(result, string.Format("Enter Full Street Address or Phone Number or Email"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(!FollowUpSettings.ConsentToEmail &&
                                                     !FollowUpSettings.ConsentToPhone &&
                                                     !FollowUpSettings.ConsentToMail &&
                                      !FollowUpSettings.ConsentToText && !FollowUpSettings.ExcludeFromFollowup);

                        return RuleResult.Assert(result, string.Format("Must Consent or Exclude From Follow Up"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !((FollowUpSettings.ConsentToEmail ||
                                                     FollowUpSettings.ConsentToPhone ||
                                                     FollowUpSettings.ConsentToMail ||
                                       FollowUpSettings.ConsentToText) && FollowUpSettings.ExcludeFromFollowup);

                        return RuleResult.Assert(result, string.Format("Cannot Consent and Exclude From Follow Up"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(!FollowUpSettings.ConsentToEmail &&
                                                     FollowUpSettings.PreferredContactMethod.Equals("Email") && !FollowUpSettings.ExcludeFromFollowup);

                        return RuleResult.Assert(result, string.Format("Must Consent To Email when Contact Preference is Email"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(!FollowUpSettings.ConsentToPhone &&
                                                     FollowUpSettings.PreferredContactMethod.Equals("Phone"));

                        return RuleResult.Assert(result, string.Format("Must Consent To Phone when Contact Preference is Phone"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(!FollowUpSettings.ConsentToMail &&
                                                     FollowUpSettings.PreferredContactMethod.Equals("Mail"));

                        return RuleResult.Assert(result, string.Format("Must Consent To Mail when Contact Preference is Mail"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(!FollowUpSettings.ConsentToText &&
                                                     FollowUpSettings.PreferredContactMethod.Equals("Text"));

                        return RuleResult.Assert(result, string.Format("Must Consent To Text when Contact Preference is Text"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(String.IsNullOrEmpty(Email.EmailAddress) &&
                                       (FollowUpSettings.PreferredContactMethod.Equals("Email") || FollowUpSettings.ConsentToEmail));

                        return RuleResult.Assert(result, string.Format("Email required when Consenting To Email or Contact Preference is Email"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(String.IsNullOrEmpty(MobilePhone.Phone) &&
                                           String.IsNullOrEmpty(HomePhone.Phone) &&
                                           String.IsNullOrEmpty(WorkPhone.Phone) &&
                                       (FollowUpSettings.PreferredContactMethod.Equals("Phone") || FollowUpSettings.ConsentToPhone));

                        return RuleResult.Assert(result, string.Format("Phone required when Consenting To Phone or Contact Preference is Phone"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(String.IsNullOrEmpty(StreetAddress.AddressLine1) &&
                                           String.IsNullOrEmpty(StreetAddress.AddressLine2) &&
                                           String.IsNullOrEmpty(StreetAddress.City) &&
                                           String.IsNullOrEmpty(StreetAddress.State) &&
                                       String.IsNullOrEmpty(StreetAddress.PostalCode) &&
                                       (FollowUpSettings.PreferredContactMethod.Equals("Mail") || FollowUpSettings.ConsentToMail));

                        return RuleResult.Assert(result, string.Format("Street Address required when Consenting To Mail or Contact Preference is Mail"));
                    });

            Validator.AddRule(() => FollowUpSettings,
                    () =>
                    {
                        var result = !(String.IsNullOrEmpty(MobilePhone.Phone) &&
                                       (FollowUpSettings.PreferredContactMethod.Equals("Text") || FollowUpSettings.ConsentToText));

                        return RuleResult.Assert(result, string.Format("Mobile Phone required when Consenting To Text or Contact Preference is Text"));
                    });

            Validator.AddRule(() => ActiveExcludeReason,
                   () =>
                   {
                       var result = !(FollowUpSettings.ExcludeFromFollowup && String.IsNullOrEmpty(FollowUpSettings.ExcludeReason));

                       return RuleResult.Assert(result, string.Format("Exclude Reason required"));
                   });
        }
    }
}
