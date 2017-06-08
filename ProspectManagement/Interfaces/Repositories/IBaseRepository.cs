using System;
namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface IBaseRepository
	{
		event EventHandler<RetrievingDataFailureEventArgs> RetrievingDataFailed;
	}
}
