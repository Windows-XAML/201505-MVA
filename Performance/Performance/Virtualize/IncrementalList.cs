using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Template10.Virtualize
{
    public class IncrementalList<T> : List<T>, ISupportIncrementalLoading, INotifyCollectionChanged, INotifyPropertyChanged
    {
        int _pageSize;
        int _currentPage = 1;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IncrementalList(IProvider<T> provider, int pageSize = 10)
        {
            this.Provider = provider;
            this._pageSize = pageSize;
        }

        bool _busy;
        public bool Busy { get { return _busy; } set { if (_busy == value) return; _busy = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Busy))); } }

        public IProvider<T> Provider { get; private set; }

        public bool HasMoreItems { get { return Provider.Count > (_pageSize * _currentPage); } }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (Busy)
                throw new Exception();
            Busy = true;
            return AsyncInfo.Run(async (c) =>
            {
                try
                {
                    var items = await Provider.LoadAsync(count, _pageSize);
                    foreach (var item in items)
                        this.Add(item.Value);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
                    return new LoadMoreItemsResult { Count = (uint)items.Count };
                }
                finally { Busy = false; }
            });
        }
    }
}
