using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.ViewModels
{
    class SampleViewModel
    {
        public SampleViewModel()
        {
            var items = Enumerable.Range(1, 10).Select(x => new Models.TodoItem
            {
                Title = string.Format("Sample Item {0}", x),
                DueDate = DateTime.Now,
                Details = "The quick brown fox jumps over the lazy dog. Now is the time for all good men to come to the aid of their country."
            });
            this.List = new Models.TodoList
            {
                Title = "Sample List",
                Items = new ObservableCollection<Models.TodoItem>(items)
            };
            this.Item = items.First();
        }

        public Models.TodoItem Item { get; set; }
        public Models.TodoList List { get; set; }
    }
}
