using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MVVMLIGHT
using ObservableObject = GalaSoft.MvvmLight.ObservableObject;
#else
using ObservableObject = Template10.Mvvm.BindableBase;
#endif

namespace Template10.Models
{
    public class DataObject : ObservableObject
    {
        private string id = Guid.NewGuid().ToString();
        /// <summary>
        /// Gets or sets the unique ID of the list.
        /// </summary>
        /// <value>
        /// The unique ID of the list.
        /// </value>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                Set(ref id, value);
            }
        }
    }
}
