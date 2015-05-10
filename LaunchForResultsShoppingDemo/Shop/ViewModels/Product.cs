using System;

namespace Shop.ViewModels
{
    public class Product : CanNotifyPropertyChanged
    {
        private string name;
        private double price;
        private bool isSelected;
        private Action onSelectionChanged;

        public Product(Action OnSelectionChangedAction = null)
        {
            this.onSelectionChanged = OnSelectionChangedAction;
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value);
            }
        }

        public double Price
        {
            get
            {
                return this.price;
            }

            set
            {
                this.SetProperty(ref this.price, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.SetProperty(ref this.isSelected, value);

                if (onSelectionChanged != null)
                {
                    onSelectionChanged.Invoke();
                }
            }
        }
    }
}