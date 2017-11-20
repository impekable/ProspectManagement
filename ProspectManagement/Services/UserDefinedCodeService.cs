using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class UserDefinedCodeService : IUserDefinedCodeService
    {
		private readonly IAuthenticator _authenticator;
		private IDialogService _dialogService;
        private readonly IUserDefinedCodeRepository _userDefinedCodeRepository;
        private List<UserDefinedCode> _prefixes;
        private List<UserDefinedCode> _suffixes;
        private List<UserDefinedCode> _contactPreferences;
        private List<UserDefinedCode> _excludeReasons;
        private List<UserDefinedCode> _states;
		private List<UserDefinedCode> _countries;

        public UserDefinedCodeService(IUserDefinedCodeRepository userDefinedCodeRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _authenticator = authenticator;
            _userDefinedCodeRepository = userDefinedCodeRepository;
        }

        public async Task<List<UserDefinedCode>> GetContactPreferenceUserDefinedCodes()
        {
            if (_contactPreferences == null)
                _contactPreferences = await getUserDefinedCodes("01", "CQ");
            return _contactPreferences;
        }

        public async Task<List<UserDefinedCode>> GetExcludeReasonUserDefinedCodes()
        {
            if (_excludeReasons == null)
                _excludeReasons = await getUserDefinedCodes("01", "26");
            return _excludeReasons;
        }

        public async Task<List<UserDefinedCode>> GetPrefixUserDefinedCodes()
        {
            if (_prefixes == null)
            {
                _prefixes = await getUserDefinedCodes("01", "W3");
                _prefixes?.Add(new UserDefinedCode() { Code = "", Description1 = " " });
            }
            return _prefixes.OrderBy(p => p.Description1).ToList();
        }

        public async Task<List<UserDefinedCode>> GetStateUserDefinedCodes()
        {
            if (_states == null)
            {
                _states = await getUserDefinedCodes("00", "S");
                _states?.Add(new UserDefinedCode() { Code = "", Description1 = " " });
            }
            return _states.OrderBy(p => p.Description1).ToList();
        }

		public async Task<List<UserDefinedCode>> GetCountryUserDefinedCodes()
		{
            if (_countries == null)
            {
                _countries = await getUserDefinedCodes("00", "CN");
                _countries?.Add(new UserDefinedCode() { Code = "", Description1 = " " });
            }
            return _countries.OrderBy(p => p.Description1).ToList();
		}

        public async Task<List<UserDefinedCode>> GetSuffixUserDefinedCodes()
        {
            if (_suffixes == null)
            {
                _suffixes = await getUserDefinedCodes("01", "W4");
                _suffixes?.Add(new UserDefinedCode() { Code = "", Description1 = " " });
            }
            return _suffixes.OrderBy(p => p.Description1).ToList();
        }

        private async Task<List<UserDefinedCode>> getUserDefinedCodes(string productCode, string group)
        {
			try
			{
				var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);

				return await _userDefinedCodeRepository.GetUserDefinedCodesAsync(productCode, group, authResult.AccessToken );
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				_dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
				return default(List<UserDefinedCode>);
			}
        }
    }
}
