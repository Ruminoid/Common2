using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Ruminoid.Common2.Collections
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public ObservableCollectionEx()
        {
        }

        public ObservableCollectionEx(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ObservableCollectionEx(List<T> list)
            : base(list)
        {
        }

        public void ForEach(Action<T> action)
        {
            foreach (T item in Items) action(item);
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range) Items.Add(item);

            OnPropertyChanged(new("Count"));
            OnPropertyChanged(new("Item[]"));
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> range)
        {
            Items.Clear();

            AddRange(range);
        }
    }
}
