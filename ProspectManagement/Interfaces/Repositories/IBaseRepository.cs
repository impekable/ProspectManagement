using System;
using Polly.Retry;

namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface IBaseRepository
	{
		event EventHandler<RetrievingDataFailureEventArgs> RetrievingDataFailed;
        AsyncRetryPolicy Policy { get; set; }
	}
}
