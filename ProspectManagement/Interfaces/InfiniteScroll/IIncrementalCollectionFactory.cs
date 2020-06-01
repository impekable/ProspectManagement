using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.InfiniteScroll
{
	public interface IIncrementalCollectionFactory
    {
        ObservableCollection<T> GetCollection<T>(Func<int, int, Task<ObservableCollection<T>>> sourceDataFunc, EventHandler<int> incrementalLoadFromBackendCompleted,
            int defaultPageSize = 10);
    }
}
