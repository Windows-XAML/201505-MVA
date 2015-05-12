
namespace Shop.ViewModels
{
    class ViewModelLocator
    {
        private MainPageViewModel mainVm;

        public MainPageViewModel Main
        {
            get
            {   
                if (mainVm == null)
                {
                    mainVm = new MainPageViewModel();
                }

                return mainVm;

            }
        }
    }
}
