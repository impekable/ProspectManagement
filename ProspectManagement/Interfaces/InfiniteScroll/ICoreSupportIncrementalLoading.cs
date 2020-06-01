using System;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.InfiniteScroll
{
	public interface ICoreSupportIncrementalLoading
    {
        Task LoadMoreItemsAsync(bool prependItem = false);
    }
}
