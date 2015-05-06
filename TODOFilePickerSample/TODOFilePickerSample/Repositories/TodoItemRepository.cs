using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODOFilePickerSample.Models;

namespace TODOFilePickerSample.Repositories
{
    public class TodoItemRepository
    {
        public Models.TodoItem Factory(string key = null, bool? complete = null, string title = null, DateTime? dueDate = null, Uri imageUri = null)
        {
            return new Models.TodoItem
            {
                Key = key ?? Guid.NewGuid().ToString(),
                IsComplete = complete ?? false,
                Title = title ?? string.Empty,
                ImageUri = imageUri,
            };
        }

        public Models.TodoItem Clone(Models.TodoItem item)
        {
            return Factory
                (
                    Guid.Empty.ToString(),
                    false,
                    item.Title,
                    item.DueDate,
                    item.ImageUri
                );
        }

        public IEnumerable<Models.TodoItem> Sample(int count = 5)
        {
            Uri[] defaultImageUris = new Uri[] 
            { new Uri("ms-appx:///Assets/DarkGray.png"),
              new Uri("ms-appx:///Assets/LightGray.png"),
               new Uri("ms-appx:///Assets/MediumGray.png"),
            };

            var random = new Random((int)DateTime.Now.Ticks);
            foreach (var item in Enumerable.Range(1, count))
            {
                yield return Factory
                    (
                        Guid.NewGuid().ToString(),
                        false,
                        "Task-" + Guid.NewGuid().ToString(),
                        DateTime.Now.AddHours(random.Next(1, 200)),
                        defaultImageUris[random.Next(0, 3)]
                    );
            }
        }
    }
}
