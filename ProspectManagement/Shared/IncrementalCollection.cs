using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.InfiniteScroll;

namespace ProspectManagement.Core.Shared
{
	public class IncrementalCollection<T> : ObservableCollection<T>, ICoreSupportIncrementalLoading
    {
        private readonly Func<int, int, Task<ObservableCollection<T>>> _sourceDataFunc;
        private int _defaultPageSize;
        private event EventHandler<int> _incrementalLoadFromBackendCompleted;

        public IncrementalCollection(Func<int, int, Task<ObservableCollection<T>>> sourceDataFunc, int defaultPageSize, EventHandler<int> incrementalLoadFromBackendCompleted)
        {
            _sourceDataFunc = sourceDataFunc;
            _defaultPageSize = defaultPageSize;
            _incrementalLoadFromBackendCompleted = incrementalLoadFromBackendCompleted;
        }

        public int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public async Task LoadMoreItemsAsync(bool prependItem = false)
        {
            ObservableCollection<T> sourceData = await _sourceDataFunc(Count, _defaultPageSize);

            if (prependItem)
            {
                //add to beginning
                foreach (T dataItem in sourceData)
                {
                    Insert(sourceData.IndexOf(dataItem), dataItem);
                }
            }
            else
            {
                foreach (T dataItem in sourceData)
                {
                    Add(dataItem);
                }
            }
            OnIncrementalLoadFromBackendCompleted(sourceData.Count);
        }

        public void OnIncrementalLoadFromBackendCompleted(int count)
        {
            _incrementalLoadFromBackendCompleted?.Invoke(null, count);
        }
    }
}
