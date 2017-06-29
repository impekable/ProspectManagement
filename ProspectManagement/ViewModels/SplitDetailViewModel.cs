using System.Windows.Input;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitDetailViewModel : BaseViewModel
    {
        private Prospect _prospect;
        private ICommand _showCobuyerTab;
        private ICommand _showTrafficCardTab;

        public ICommand ShowCobuyerTab
        {
            get
            {
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<CobuyerViewModel>(_prospect)));
            }
        }
		public ICommand ShowTrafficCardTab
		{
			get
			{
				return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<TrafficCardViewModel>(_prospect)));
			}
		}

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => Prospect);
            }
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public async void Init(Prospect prospect)
        {
            Prospect = prospect;
        }
    }


}
