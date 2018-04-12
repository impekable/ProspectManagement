using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Collections.ObjectModel;
using MvvmValidation;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Extensions;
using System.Threading.Tasks;
using System.Linq;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Messages;
using MvvmCross.Core.Navigation;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace ProspectManagement.Core.ViewModels
{
    public class CobuyerDetailViewModel : BaseViewModel, IMvxViewModel<Cobuyer>
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private UserDefinedCode _originalPickerValue;

        private Prospect _prospect;
        private Cobuyer _cobuyer;
        private Cobuyer _originalCobuyer;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _nickName;
        private StreetAddress _streetAddress;
        private PhoneNumber _mobilePhoneNumber;
        private PhoneNumber _workPhoneNumber;
        private PhoneNumber _homePhoneNumber;
        private Email _email;
        private User _user;
        private bool _addressSameAsBuyer;

        private readonly IAuthenticator _authenticator;
        private readonly IDialogService _dialogService;
        private readonly IEmailValidationService _emailValidationService;
        private readonly IPhoneNumberValidationService _phoneNumberValidationService;
        private readonly IStreetValidationService _streetValidationService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;
        private readonly ICobuyerService _cobuyerService;
        private readonly IProspectService _prospectService;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;

        protected IMvxMessenger Messenger;

        private ICommand _saveCommand;
        private ICommand _closeCommand;

        private ObservableCollection<UserDefinedCode> _prefixes;
        private ObservableCollection<UserDefinedCode> _suffixes;
        private ObservableCollection<UserDefinedCode> _states;
        private ObservableCollection<UserDefinedCode> _countries;

        private UserDefinedCode _activePrefix;
        private UserDefinedCode _activeSuffix;
        private UserDefinedCode _activeState;
        private UserDefinedCode _activeCountry;

        private string _firstNameError;
        private string _lastNameError;
        private string _emailAddressError;
        private string _mobilePhoneNumberError;
        private string _workPhoneNumberError;
        private string _homePhoneNumberError;

        public UserDefinedCode OriginalPickerValue
        {
            get { return _originalPickerValue; }
            set
            {
                _originalPickerValue = value;
                RaisePropertyChanged(() => OriginalPickerValue);
            }
        }

        public UserDefinedCode ActivePrefix
        {
            get { return _activePrefix; }
            set
            {
                _activePrefix = value;
                RaisePropertyChanged(() => ActivePrefix);
            }
        }

        public UserDefinedCode ActiveSuffix
        {
            get { return _activeSuffix; }
            set
            {
                _activeSuffix = value;
                RaisePropertyChanged(() => ActiveSuffix);
            }
        }

        public UserDefinedCode ActiveState
        {
            get { return _activeState; }
            set
            {
                _activeState = value;
                StreetAddress.State = _activeState == null ? String.Empty : _activeState.Code;
                RaisePropertyChanged(() => ActiveState);
            }
        }

        public UserDefinedCode ActiveCountry
        {
            get { return _activeCountry; }
            set
            {
                _activeCountry = value;
                StreetAddress.Country = _activeCountry == null ? String.Empty : _activeCountry.Code;
                RaisePropertyChanged(() => ActiveCountry);
                RaisePropertyChanged(() => ForeignState);
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

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new MvxCommand(async () =>
                {
                    var cobuyerUpdated = false;
                    var cobuyerAdded = false;

                    await ValidateAsync();
                    if (IsValid.GetValueOrDefault())
                    {
                        Cobuyer.FirstName = FirstName;
                        Cobuyer.LastName = LastName;
                        Cobuyer.MiddleName = MiddleName;
                        Cobuyer.NickName = NickName;
                        Cobuyer.NamePrefix = _activePrefix != null ? _activePrefix.Description1 : String.Empty;
                        Cobuyer.NameSuffix = _activeSuffix != null ? _activeSuffix.Description1 : String.Empty;
                        Cobuyer.StreetAddress = !String.IsNullOrEmpty(StreetAddress.AddressLine1) ||
                                                       !String.IsNullOrEmpty(StreetAddress.AddressLine2) ||
                                                       !String.IsNullOrEmpty(StreetAddress.City) ||
                                                       !String.IsNullOrEmpty(StreetAddress.State) ||
                                                        !String.IsNullOrEmpty(StreetAddress.PostalCode) ? StreetAddress : null;
                        Cobuyer.MobilePhoneNumber = MobilePhone;
                        Cobuyer.WorkPhoneNumber = WorkPhone;
                        Cobuyer.HomePhoneNumber = HomePhone;
                        Cobuyer.Email = Email;
                        Cobuyer.AddressSameAsBuyer = AddressSameAsBuyer;

                        if (Cobuyer.CobuyerAddressNumber > 0)
                            cobuyerUpdated = await _cobuyerService.UpdateCobuyerAsync(Cobuyer);
                        else
                        {
                            var newCobuyer = await _cobuyerService.AddCobuyerToProspectAsync(Cobuyer.ProspectAddressNumber, Cobuyer);
                            cobuyerAdded = newCobuyer != null;
                            Cobuyer.CobuyerAddressNumber = newCobuyer != null ? newCobuyer.CobuyerAddressNumber : 0;
                        }

                    }
                    _hideAlertInteraction.Raise();

                    if (cobuyerUpdated)
                    {
                        Analytics.TrackEvent("Cobuyer Updated", new Dictionary<string, string>
                        {
                            {"Community", Cobuyer.Prospect.ProspectCommunity.CommunityNumber + " " + Cobuyer.Prospect.ProspectCommunity.Community.Description},
                            {"SalesAssociate", Cobuyer.Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Cobuyer.Prospect.ProspectCommunity.SalespersonName},
                            {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                        });
                        Messenger.Publish(new CobuyerChangedMessage(this) { UpdatedCobuyer = new Cobuyer() { CobuyerAddressNumber = Cobuyer.CobuyerAddressNumber, FirstName = Cobuyer.FirstName, LastName = Cobuyer.LastName } });

                        await _navigationService.Close(this);
                    }
                    else if (cobuyerAdded)
                    {
                        Analytics.TrackEvent("Cobuyer Added", new Dictionary<string, string>
                        {
                            {"Community", Cobuyer.Prospect.ProspectCommunity.CommunityNumber + " " + Cobuyer.Prospect.ProspectCommunity.Community.Description},
                            {"SalesAssociate", Cobuyer.Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Cobuyer.Prospect.ProspectCommunity.SalespersonName},
                            {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                        });
                        Messenger.Publish(new CobuyerAddedMessage(this) { AddedCobuyer = Cobuyer });

                        await _navigationService.Close(this);
                    }
                    else
                    {
                        Cobuyer.FirstName = _originalCobuyer.FirstName;
                        Cobuyer.LastName = _originalCobuyer.LastName;
                        Cobuyer.MiddleName = _originalCobuyer.MiddleName;
                        Cobuyer.NickName = _originalCobuyer.NickName;
                        Cobuyer.NamePrefix = _originalCobuyer.NamePrefix;
                        Cobuyer.NameSuffix = _originalCobuyer.NameSuffix;
                        Cobuyer.StreetAddress = _originalCobuyer.StreetAddress.ShallowCopy();
                        Cobuyer.MobilePhoneNumber = _originalCobuyer.MobilePhoneNumber.ShallowCopy();
                        Cobuyer.HomePhoneNumber = _originalCobuyer.HomePhoneNumber.ShallowCopy();
                        Cobuyer.WorkPhoneNumber = _originalCobuyer.WorkPhoneNumber.ShallowCopy();
                        Cobuyer.Email = _originalCobuyer.Email.ShallowCopy();
                        Cobuyer.AddressSameAsBuyer = _originalCobuyer.AddressSameAsBuyer;
                    }
                }));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () => await _navigationService.Close(this)));
            }
        }

        public Cobuyer Cobuyer
        {
            get { return _cobuyer; }
            set
            {
                _cobuyer = value;
                RaisePropertyChanged(() => Cobuyer);
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

        public bool EnableAddressFields
        {
            get { return !(AddressSameAsBuyer); }
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

        public Boolean AddressSameAsBuyer
        {
            get { return _addressSameAsBuyer; }
            set
            {
                _addressSameAsBuyer = value;
                if (_addressSameAsBuyer)
                    UpdateCobuyerAddressWithProspectAddress();
                RaisePropertyChanged(() => AddressSameAsBuyer);
                RaisePropertyChanged(() => EnableAddressFields);
                Validator.ValidateAsync(nameof(AddressSameAsBuyer));
            }
        }

        public CobuyerDetailViewModel(IAuthenticator authenticator, IMvxMessenger messenger, IProspectService prospectService, IUserDefinedCodeService userDefinedCodeService, IStreetValidationService streetValidationService, IDialogService dialogService, IPhoneNumberValidationService phoneNumberValidationService, IEmailValidationService emailValidationService, ICobuyerService cobuyerService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _dialogService = dialogService;
            _phoneNumberValidationService = phoneNumberValidationService;
            _streetValidationService = streetValidationService;
            _emailValidationService = emailValidationService;
            _cobuyerService = cobuyerService;
            _userDefinedCodeService = userDefinedCodeService;
            _authenticator = authenticator;
            _prospectService = prospectService;
            _navigationService = navigationService;
            _userService = userService;

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;

            Messenger.Subscribe<RefreshMessage>(async message => await _navigationService.Close(this), MvxReference.Strong);
        }

        public void Prepare(Cobuyer cobuyer)
        {
            Cobuyer = cobuyer;
            if (Cobuyer.CobuyerAddressNumber > 0)
            {
                FirstName = Cobuyer.FirstName;
                LastName = Cobuyer.LastName;
                MiddleName = Cobuyer.MiddleName;
                NickName = Cobuyer.NickName;
                StreetAddress = Cobuyer.StreetAddress == null ? new StreetAddress() : Cobuyer.StreetAddress.ShallowCopy();
                MobilePhone = Cobuyer.MobilePhoneNumber == null ? new PhoneNumber() : Cobuyer.MobilePhoneNumber.ShallowCopy();
                WorkPhone = Cobuyer.WorkPhoneNumber == null ? new PhoneNumber() : Cobuyer.WorkPhoneNumber.ShallowCopy();
                HomePhone = Cobuyer.HomePhoneNumber == null ? new PhoneNumber() : Cobuyer.HomePhoneNumber.ShallowCopy();
                Email = Cobuyer.Email == null ? new Email() : Cobuyer.Email.ShallowCopy();
                _addressSameAsBuyer = Cobuyer.AddressSameAsBuyer;
            }
            else
            {
                StreetAddress = new StreetAddress();
                MobilePhone = new PhoneNumber();
                WorkPhone = new PhoneNumber();
                HomePhone = new PhoneNumber();
                Email = new Email();
            }
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();

            _originalCobuyer = Cobuyer.ShallowCopy();
            _originalCobuyer.StreetAddress = StreetAddress.ShallowCopy();
            _originalCobuyer.MobilePhoneNumber = MobilePhone.ShallowCopy();
            _originalCobuyer.WorkPhoneNumber = WorkPhone.ShallowCopy();
            _originalCobuyer.HomePhoneNumber = HomePhone.ShallowCopy();
            _originalCobuyer.Email = Email.ShallowCopy();

            Prefixes = (await _userDefinedCodeService.GetPrefixUserDefinedCodes()).ToObservableCollection();
            ActivePrefix = Prefixes.FirstOrDefault(p => p.Description1 == Cobuyer.NamePrefix);

            Suffixes = (await _userDefinedCodeService.GetSuffixUserDefinedCodes()).ToObservableCollection();
            ActiveSuffix = Suffixes.FirstOrDefault(p => p.Description1 == Cobuyer.NameSuffix);

            States = (await _userDefinedCodeService.GetStateUserDefinedCodes()).ToObservableCollection();
            ActiveState = Cobuyer.StreetAddress != null ? States.FirstOrDefault(p => p.Code == Cobuyer.StreetAddress.State) : null;

            Countries = (await _userDefinedCodeService.GetCountryUserDefinedCodes()).ToObservableCollection();
            ActiveCountry = Cobuyer.StreetAddress != null ? Countries.FirstOrDefault(p => p.Code == Cobuyer.StreetAddress.Country) : null;
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
        }

        private void UpdateCobuyerAddressWithProspectAddress()
        {
            if (Cobuyer.Prospect.StreetAddress != null)
            {
                StreetAddress = Cobuyer.Prospect.StreetAddress.ShallowCopy();
                ActiveState = States.FirstOrDefault(p => p.Code == StreetAddress.State);
                ActiveCountry = Countries.FirstOrDefault(p => p.Code == StreetAddress.Country);
            }

            if (Cobuyer.Prospect.HomePhoneNumber != null)
            {
                HomePhoneNumber = Cobuyer.Prospect.HomePhoneNumber.Phone;
            }
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


            Validator.AddAsyncRule(() => Email,
                   async () =>
                    {
                        var verifyViaService = (!Email.EmailVerified || (Email.EmailAddress != _originalCobuyer.Email.EmailAddress))
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
                        var verifyViaService = (!MobilePhone.PhoneVerified || (MobilePhone.Phone != _originalCobuyer.MobilePhoneNumber.Phone))
                                                            && !String.IsNullOrEmpty(MobilePhone.Phone);
                        var result = !verifyViaService ? true :
                                           await _phoneNumberValidationService.Validate(MobilePhone);

                        return RuleResult.Assert(result,
                            string.Format("Mobile Phone number is invalid"));
                    });

            Validator.AddAsyncRule(() => HomePhone,
                   async () =>
                    {
                        var verifyViaService = (!HomePhone.PhoneVerified || (HomePhone.Phone != _originalCobuyer.HomePhoneNumber.Phone))
                                                            && !String.IsNullOrEmpty(HomePhone.Phone);
                        var result = !verifyViaService ? true :
                                           await _phoneNumberValidationService.Validate(HomePhone);

                        return RuleResult.Assert(result,
                            string.Format("Home Phone number is invalid"));
                    });

            Validator.AddAsyncRule(() => WorkPhone,
                   async () =>
                    {
                        var verifyViaService = (!WorkPhone.PhoneVerified || (WorkPhone.Phone != _originalCobuyer.WorkPhoneNumber.Phone))
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
                                                (StreetAddress.AddressLine1 != _originalCobuyer.StreetAddress.AddressLine1) ||
                                                (StreetAddress.AddressLine2 != _originalCobuyer.StreetAddress.AddressLine2) ||
                                                (StreetAddress.City != _originalCobuyer.StreetAddress.City) ||
                                                (StreetAddress.State != _originalCobuyer.StreetAddress.State) ||
                                                (StreetAddress.PostalCode != _originalCobuyer.StreetAddress.PostalCode))
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


            Validator.AddRule(() => Cobuyer,
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
        }
    }
}
