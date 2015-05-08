using System;
using System.Linq;
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

        public override async Task OnActivatedAsync(IActivatedEventArgs e)
        {
            // If the app was launched via some other mechanism than a Voice Command, exit. If 
            // the app is able to act as a share target, or handle various file types, etc,  
            // then developers should handle these cases here. 
            if (e.Kind != ActivationKind.VoiceCommand)
            {
                return;
            }

            // The arguments can represent many different activation types. Cast it so we can get the 
            // parameters we care about out. 
            var commandArgs = e as VoiceCommandActivatedEventArgs;
            var reco = commandArgs.Result;

            // Get the name of the voice command and the text spoken. See AdventureWorksCommands.xml for 
            // the <Command> tags this can be filled with. 
            string commandName = reco.RulePath[0];
            string listName = reco.SemanticInterpretation.Properties["listName"].FirstOrDefault();

            // Multiple different voice commands may be supported, switch between them (The voiceCommandName 
            switch (commandName)
            {
                case "showList":
                    NavigationService.Navigate(typeof(Views.MainPage), listName);
                    break;
                default:
                    // If we can't determine what page to launch, go to the default entry point. 
                    NavigationService.Navigate(typeof(Views.MainPage));
                    break;
            }
            await base.OnActivatedAsync(e);
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
