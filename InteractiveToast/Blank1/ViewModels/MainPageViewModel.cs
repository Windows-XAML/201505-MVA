using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace Blank1.ViewModels
{
    public class MainPageViewModel : Mvvm.ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                foreach (var item in Enumerable.Range(1, 20))
                {
                    this.Items.Add(Guid.NewGuid().ToString());
                }
            }
            else
            {
                ApplicationData.Current.DataChanged += (s, e) => Update();
                Update();
            }
        }

        public override void OnNavigatedTo(string parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            this.Items.Add("Init");
        }

        private void Update()
        {
            this.Items.Clear();
            var container = ApplicationData.Current.LocalSettings.CreateContainer("Values", ApplicationDataCreateDisposition.Always);
            foreach (var item in container.Values)
            {
                this.Items.Add(item.Value?.ToString() ?? "Empty");
            }
        }

        public ObservableCollection<string> Items { get; private set; } = new ObservableCollection<string>();
    }
}
