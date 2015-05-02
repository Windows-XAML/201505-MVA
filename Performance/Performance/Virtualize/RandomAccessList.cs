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
    using System.Threading.Tasks;

    public class RandomAccessList<T> : ObservableCollection<object>, IObservableVector<object>
    {
        public event VectorChangedEventHandler<object> VectorChanged;
        Dictionary<int, T> _cache = new Dictionary<int, T>();
        Stack<int> _chunks = new Stack<int>();
        public int _previousIndex = 1;
        public int _cacheLimit = 100;
        public int _takeSize;

        #region

        public IProvider<T> Provider { get; private set; }
        public new int Count { get; set; } = 0;

        bool _busy;
        public bool Busy { get { return _busy; } set { if (_busy == value) return; _busy = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(Busy))); } }

        #endregion

        public RandomAccessList(IProvider<T> provider, int takeSize = 20)
        {
            Debug.WriteLine("ObservableVector.Constructor");
            this._takeSize = takeSize;
            this.Provider = provider;
            var x = this[1];
        }

        public new object this[int index]
        {
            get
            {
                Debug.WriteLine("ObservableVector.Index {0}", index);

                T value;
                if (!_cache.TryGetValue(_previousIndex = index, out value))
                {
                    Task.Run((Func<Task>)(async () =>
                    {
                        Debug.WriteLine("ObservableVector.StartLoadAsync {0}", index);

                        if (_chunks.Contains(index))
                            _chunks.Push(index);
                        if (Busy)
                            return;
                        try
                        {
                            var i = (_chunks.Any()) ? _chunks.Pop() : index;
                            if (Math.Abs(i - _previousIndex) >= _cacheLimit)
                                return;
                            Busy = true;

                            var items = await Provider.LoadAsync((uint)i, _takeSize);
                            foreach (var item in items)
                            {
                                if (_cache.ContainsKey(item.Key))
                                    _cache[item.Key] = item.Value;
                                else
                                    _cache.Add(item.Key, item.Value);
                                base.Add((T)item.Value);
                                var current = (uint)IndexOf(item.Value);
                                VectorChanged?.Invoke(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = current });
                            }
                        }
                        catch { Debugger.Break(); }
                        finally { Busy = false; }
                    }));
                }
                return value;
            }
            set { throw new NotImplementedException(); }
        }

        private class VectorChangedEventArgs : IVectorChangedEventArgs
        {
            public CollectionChange CollectionChange { get; set; }
            public uint Index { get; set; }
        }
    }

}
