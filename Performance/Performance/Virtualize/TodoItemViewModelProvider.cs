using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Repositories;
using Template10.ViewModels;

namespace Template10.Virtualize
{
    public class TodoItemViewModelProvider : IProvider<TodoItemViewModel>
    {
        private TodoItemRepository _repository;
        public event PropertyChangedEventHandler PropertyChanged;

        public TodoItemViewModelProvider()
        {
            this._repository = new Repositories.TodoItemRepository();

            // hard coded for demo
            this.Count = 1000;
        }

        public int Count { get; set; }

        bool _busy;
        public bool Busy { get { return _busy; } set { if (_busy == value) return; _busy = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Busy))); } }

        public async Task<Dictionary<int, TodoItemViewModel>> LoadAsync(uint start, int size)
        {
            // constrain to count
            size = (int)start + size;
            if (size > Count) size = Count;

            var dictionary = new Dictionary<int, TodoItemViewModel>();
            foreach (var index in Enumerable.Range((int)start, size))
            {
                // fake delay for demo
                await Task.Delay(10);

                // fake data for demo
                var model = _repository.Sample(1).First();
                var viewmodel = new ViewModels.TodoItemViewModel(model);
                dictionary.Add(index, viewmodel);
            }
            return dictionary;
        }
    }
}
