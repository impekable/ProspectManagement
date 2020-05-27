using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;

namespace ProspectManagement.Core.Services
{
    public class TwilioService: ITwilioService
    {
        private readonly ITwilioRepository _twilioRepository;
        private IDialogService _dialogService;
      
        public TwilioService(ITwilioRepository twilioRepository, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _twilioRepository = twilioRepository;
            _twilioRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem." + e.RetrievingDataFailureMessage, "Oops", "Close");
            };

        }

        public async Task<string> GetClientToken(string userId)
        {
            var token = await _twilioRepository.GetClientToken(userId);
            return token;
        }
    }
}
