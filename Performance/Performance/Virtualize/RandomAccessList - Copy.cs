namespace Template10.Virtualize
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Windows.Foundation.Collections;
    using System.Collections.Specialized;

    public class RandomAccessList<T> : Collection<object>, INotifyPropertyChanged, IObservableVector<object>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event VectorChangedEventHandler<object> VectorChanged;

        Dictionary<int, T> _cache = new Dictionary<int, T>();
        Stack<int> _chunks = new Stack<int>();
        public int _previousIndex = 0;
        public int _cacheLimit = 100;
        public int _takeSize;

        public IProvider<T> Provider { get; private set; }
        public new int Count { get; set; } = 0;

        bool _busy;
        public bool Busy { get { return _busy; } set { if (_busy == value) return; _busy = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Busy))); } }

        public RandomAccessList(IProvider<T> provider, int takeSize = 20)
        {
            Debug.WriteLine("ObservableVector.Constructor");
            this._takeSize = takeSize;
            this.Provider = provider;
        }

        public new object this[int index]
        {
            get
            {
                Debug.WriteLine("ObservableVector.Index {0}", index);

                T value;
                if (!_cache.TryGetValue(_previousIndex = index, out value))
                    StartLoadAsync(index);
                return value;
            }
            set { throw new NotImplementedException(); }
        }

        private async void StartLoadAsync(int start)
        {
            Debug.WriteLine("ObservableVector.StartLoadAsync {0}", start);

            if (_chunks.Contains(start))
                _chunks.Push(start);
            if (Busy)
                return;
            try
            {
                var index = (_chunks.Any()) ? _chunks.Pop() : start;
                if (Math.Abs(index - _previousIndex) >= _cacheLimit)
                    return;
                Busy = true;

                var items = await Provider.LoadAsync((uint)start, _takeSize);
                foreach (var item in items)
                {
                    _cache[item.Key] = item.Value;
                    var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item.Value, default(object), item.Key);
                    CollectionChanged?.Invoke(this, args);
                    InsertItem(item.Key, (T)item.Value);
                }
            }
            finally { Busy = false; }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            RaiseChanged(true, true, CollectionChange.Reset, 0);
        }

        protected override void InsertItem(int index, object item)
        {
            base.InsertItem(index, item);
            RaiseChanged(true, true, CollectionChange.ItemInserted, (uint)index);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            RaiseChanged(true, true, CollectionChange.ItemRemoved, (uint)index);
        }

        protected override void SetItem(int index, object item)
        {
            base.SetItem(index, (T)item);
            RaiseChanged(false, true, CollectionChange.ItemChanged, (uint)index);
        }

        void RaiseChanged(bool count, bool items, CollectionChange change, uint index)
        {
            if (count) this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            if (items) this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            VectorChanged?.Invoke(this, new VectorChangedEventArgs { CollectionChange = change, Index = index });
        }

        public new int IndexOf(object item) { throw new NotImplementedException(); }
        public new void Insert(int index, object item) { throw new NotImplementedException(); }
        public new void Add(object item) { throw new NotImplementedException(); }
        public new bool Contains(object item) { throw new NotImplementedException(); }
        public new void CopyTo(object[] array, int arrayIndex) { throw new NotImplementedException(); }
        public new bool Remove(object item) { throw new NotImplementedException(); }
        IEnumerator<object> IEnumerable<object>.GetEnumerator() { throw new NotImplementedException(); }

        private class VectorChangedEventArgs : IVectorChangedEventArgs
        {
            public CollectionChange CollectionChange { get; set; }
            public uint Index { get; set; }
        }
    }

}
