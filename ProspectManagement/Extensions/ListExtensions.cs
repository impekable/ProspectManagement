using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Extensions
{
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> items)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in items)
            {
                collection.Add(item);
            }
            return collection;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in items)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}
