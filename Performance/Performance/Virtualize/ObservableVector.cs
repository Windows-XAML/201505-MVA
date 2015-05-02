
namespace Template10.Virtualize
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Windows.Foundation.Collections;

    //NOTE: This a concrete implementation of IObservableVector<T> which will raise change notifications
    //NOTE: Currently ObservableCollection does not support change notifications 
    // so in the interim please use the extension method ToObservableVector<T>
    public class ObservableVector<T> : IObservableVector<T>, IList<T>, IEnumerable<T>, IEnumerable
    {
        private IList<T> _internalCollection;
        private ReadOnlyCollection<T> _readOnlyCollection;

        public ObservableVector(INotifyCollectionChanged list)
        {
            if (list is IList<T>)
            {
                _internalCollection = list as IList<T>;
                _readOnlyCollection = new ReadOnlyCollection<T>((IList<T>)list);
            }
            else
            {
                throw new Exception("Must implement IList<T>");
            }
        }

        #region IList<object> Members

        public int IndexOf(T item)
        {
            return _internalCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalCollection.Insert(index, item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)index });
            }
        }

        public void RemoveAt(int index)
        {
            _internalCollection.RemoveAt(index);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)index });
            }
        }

        public T this[int index]
        {
            get
            {
                return _internalCollection[index];
            }
            set
            {
                _internalCollection[index] = value;

                if (VectorChanged != null)
                {
                    VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemChanged, Index = (uint)index });
                }
            }
        }

        #endregion

        #region ICollection<object> Members

        public void Add(T item)
        {
            _internalCollection.Add(item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public bool Contains(T item)
        {
            return _internalCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _internalCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            int index = _internalCollection.IndexOf(item);
            bool retVal = _internalCollection.Remove(item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)index });
            }

            return retVal;
        }

        #endregion

        #region IIterable<object> Members

        public Windows.Foundation.Collections.IIterator<object> First()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        #endregion

        #region IEnumerable<object> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        #endregion

        #region IObservableVector<object> Members

        public event VectorChangedEventHandler<T> VectorChanged;

        #endregion

        #region IVector<object> Members

        public void Append(T value)
        {
            _internalCollection.Add(value);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public void Clear()
        {
            _internalCollection.Clear();

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.Reset, Index = 0 });
            }
        }

        public object GetAt(uint index)
        {
            return _internalCollection[(int)index];
        }

        public uint GetMany(uint startIndex, object[] items)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> GetView()
        {
            return _readOnlyCollection;
        }

        public bool IndexOf(T value, out uint index)
        {
            index = (uint)_internalCollection.IndexOf(value);
            return true;
        }

        public void InsertAt(uint index, T value)
        {
            _internalCollection.Insert((int)index, value);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = index });
            }
        }

        public void RemoveAt(uint index)
        {
            _internalCollection.RemoveAt((int)index);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = index });
            }
        }

        public void RemoveAtEnd()
        {
            _internalCollection.RemoveAt(_internalCollection.Count - 1);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public void ReplaceAll(object[] items)
        {
            throw new NotImplementedException();
        }

        public void SetAt(uint index, T value)
        {
            _internalCollection[(int)index] = value;

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemChanged, Index = index });
            }
        }

        public uint Size
        {
            get
            {
                return (uint)_internalCollection.Count;
            }
        }

        #endregion
    }

    public class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        #region IVectorChangedEventArgs Members

        public CollectionChange CollectionChange
        {
            get;
            set;
        }

        public uint Index
        {
            get;
            set;
        }

        #endregion
    }

    public static class ObservableVectorExtensionClass
    {
        public static ObservableVector<T> ToObservableVector<T>(this INotifyCollectionChanged s)
        {
            return new ObservableVector<T>(s);
        }
    }
}

