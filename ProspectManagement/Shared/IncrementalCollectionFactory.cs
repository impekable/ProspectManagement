using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.InfiniteScroll;

namespace ProspectManagement.Core.Shared
{
	public class IncrementalCollectionFactory : IIncrementalCollectionFactory
    {
        public ObservableCollection<T> GetCollection<T>(Func<int, int, Task<ObservableCollection<T>>> sourceDataFunc, EventHandler<int> incrementalLoadFromBackendCompleted, int defaultPageSize = 10)
        {
            return new IncrementalCollection<T>(sourceDataFunc, defaultPageSize, incrementalLoadFromBackendCompleted);
        }
    }
}
