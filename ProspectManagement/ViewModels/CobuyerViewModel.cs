using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class CobuyerViewModel: BaseViewModel
    {
		private Prospect _prospect;
		private ICommand _showDetailTab;
		private ICommand _showTrafficCardTab;
        private ICommand _showCobuyerDetail;
        private Cobuyer _cobuyer;

		public ICommand ShowDetailTab
		{
			get
			{
                return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<SplitDetailViewModel>(_prospect) ));
			}
		}
		public ICommand ShowTrafficCardTab
		{
			get
			{
				return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<TrafficCardViewModel>(_prospect)));
			}
		}
		public ICommand ShowCobuyerDetail
		{
			get
			{
				return _showCobuyerDetail ?? (_showCobuyerDetail = new MvxCommand<Cobuyer>((cobuyer) => ShowViewModel<CobuyerDetailViewModel>(_cobuyer)));
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
