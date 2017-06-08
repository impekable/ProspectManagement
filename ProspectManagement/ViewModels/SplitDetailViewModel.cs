using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class SplitDetailViewModel : BaseViewModel
    {
        private Prospect _prospect;

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
