using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Virtualize;

namespace Template10.ViewModels
{
    public class IncrementalAccessPageViewModel : Mvvm.ViewModelBase
    {
        public IncrementalAccessPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var repo = new Repositories.TodoItemRepository();
                foreach (var item in repo.Sample())
                {
                    this.Items.Add(new ViewModels.TodoItemViewModel(item));
                }
            }
            else
            {
                this.Items.Provider.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName.Equals(nameof(Busy)))
                        this.Busy = this.Items.Provider.Busy;
                };
            }
        }

        bool _busy = false;
        public bool Busy { get { return _busy; } set { Set(ref _busy, value); } }

        IncrementalList<TodoItemViewModel> _items = new IncrementalList<TodoItemViewModel>(new TodoItemViewModelProvider());
        public IncrementalList<TodoItemViewModel> Items { get { return _items; } set { Set(ref _items, value); } }
    }
}
