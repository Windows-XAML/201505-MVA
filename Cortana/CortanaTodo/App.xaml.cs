using System;
using System.Threading.Tasks;
using Template10.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace CortanaTodo
{
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() : base()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Installs the Voice Command Definitions.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        /// <remarks>
        /// Since there's no way to test that the VCD has been imported or that it's the current version it's okay to do this on application load.
        /// </remarks>
        private async Task InstallVoiceCommandsAsync()
        {
            try
            {
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"VoiceCommands.xml");
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }
        }

        public override Task OnLaunchedAsync(ILaunchActivatedEventArgs e)
        {
            // Navigate to main page
            this.NavigationService.Navigate(typeof(Views.MainPage));

            // Register voice commands async
            return InstallVoiceCommandsAsync();
        }
    }
}
