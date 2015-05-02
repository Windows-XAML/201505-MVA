namespace Template10.ViewModels
{
    using Template10.Virtualize;

    class RandomAccessPageViewModel : Mvvm.ViewModelBase
    {
        public RandomAccessPageViewModel()
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

        RandomAccessList<TodoItemViewModel> _items = new RandomAccessList<TodoItemViewModel>(new TodoItemViewModelProvider());
        public RandomAccessList<TodoItemViewModel> Items { get { return _items; } set { Set(ref _items, value); } }
    }
}

