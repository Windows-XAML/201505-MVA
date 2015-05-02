using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template10.Virtualize
{
    public interface IProvider<T> : INotifyPropertyChanged
    {
        int Count { get; }
        bool Busy { get; set; }

        Task<Dictionary<int, T>> LoadAsync(uint start, int size);
    }
}
