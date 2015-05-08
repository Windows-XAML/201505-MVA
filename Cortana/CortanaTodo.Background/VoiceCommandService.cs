using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CortanaTodo.Models;
using CortanaTodo.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace CortanaTodo.Background
{
    /// <summary>
    /// The VoiceCommandService implements the entrypoint for all headless voice commands
    /// invoked via Cortana. The individual commands supported are described in the
    /// VoiceCommands.xml VCD file in the CortanaTodo project. The service
    /// entrypoint is defined in the Package Manifest (See section uap:Extension in 
    /// CortanaTodo:Package.appxmanifest)
    /// </summary>
    public sealed class VoiceCommandService : IBackgroundTask
    {
        #region Constants
        /// <summary>
        /// The number of characters a list name can be off by and still be considered a close match.
        /// </summary>
        private const int CloseMatchOffset = 4;
        #endregion // Constants

        #region Member Variables
        /// <summary>
        /// the service connection is maintained for the lifetime of a cortana session, once a voice command
        /// has been triggered via Cortana.
        /// </summary>
        private VoiceCommandServiceConnection voiceConnection;

        /// <summary>
        /// Lifetime of the background service is controlled via the BackgroundTaskDeferral object, including
        /// registering for cancellation events, signalling end of execution, etc. Cortana may terminate the 
        /// background service task if it loses focus, or the background task takes too long to provide.
        /// 
        /// Background tasks can run for a maximum of 30 seconds.
        /// </summary>
        private BackgroundTaskDeferral serviceDeferral;
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Demonstrates providing the user with a choice between multiple items.
        /// </summary>
        /// <param name="items">The set of items to choose between</param>
        /// <param name="titleFunc">
        /// A function that returns the title of the item.
        /// </param>
        /// <param name="descriptionFunc">
        /// A function that returns the description of the item.
        /// </param>
        /// <param name="message">The initial disambiguation message</param>
        /// <param name="secondMessage">Repeat prompt retry message</param>
        /// <returns></returns>
        private async Task<T> Disambiguate<T>(IEnumerable<T> items, Func<T,string> titleFunc, Func<T, string> descriptionFunc, string message, string secondMessage)
        {
            // Create the first prompt message.
            var userPrompt = new VoiceCommandUserMessage();
            userPrompt.DisplayMessage =
                userPrompt.SpokenMessage = message;

            // Create a re-prompt message if the user responds with an out-of-grammar response.
            var userReprompt = new VoiceCommandUserMessage();
            userPrompt.DisplayMessage =
                userPrompt.SpokenMessage = secondMessage;

            // Create items for each item. Ideally, should be limited to a small number of items.
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            foreach (T item in items)
            {
                var destinationTile = new VoiceCommandContentTile();

                // Use a generic background image. This can be fetched from a service call, potentially, but
                // be aware of network latencies and ensure Cortana does not time out.
                destinationTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                destinationTile.Image = await Package.Current.InstalledLocation.GetFileAsync("CortanaTodo.Background\\Images\\TodoIcon.png");

                // The AppContext can be any arbitrary object, and will be maintained for the
                // response.
                destinationTile.AppContext = item;

                // Format title and description
                destinationTile.Title = titleFunc(item);
                destinationTile.TextLine1 = descriptionFunc(item);

                // Add
                destinationContentTiles.Add(destinationTile);
            }

            // Cortana will handle re-prompting if the user does not provide a valid response.
            var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt, destinationContentTiles);

            // If cortana is dismissed in this operation, null will be returned.
            var result = await voiceConnection.RequestDisambiguationAsync(response);
            if (result != null)
            {
                return (T)result.SelectedItem.AppContext;
            }

            return default(T);
        }

        /// <summary>
        /// Attempts to find an item with the specified name, disambiguating if necessary.
        /// </summary>
        /// <param name="list">
        /// The list containing the item to find.
        /// </param>
        /// <param name="itemName">
        /// The name of the item to find.
        /// </param>
        /// <param name="requireExact">
        /// <c>true</c> if an exact match is required; otherwise <c>false</c> to allow disambiguation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the list if found; otherwise <see langword = "null" />.
        /// </returns>
        private async Task<TodoItem> FindItemAsync(TodoList list, string itemName, bool requireExact)
        {
            // Try to find the item that matches exactly
            var item = list.Items.Where(i => i.Title.Equals(itemName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            // If no exact match was found and it's allowed, disambiguate
            if ((item == null) && (!requireExact))
            {
                var closeMatches = list.Items.Where(i => Math.Abs(i.Title.CompareTo(itemName)) < CloseMatchOffset);
                if (closeMatches.Count() > 1)
                {
                    // Attempt to disambiguate
                    item = await Disambiguate(
                        closeMatches, // Which items?
                        (l) => l.Title, // Title
                        (l) => "", // Description
                        "Which item?", // Message 1
                        "I'm sorry, which item?" // Message 2
                        );
                }
                else
                {
                    // Only one or no lists are close matches
                    item = closeMatches.FirstOrDefault();
                }
            }

            // Return whatever we've found
            return item;
        }

        /// <summary>
        /// Attempts to find a list with the specified name, disambiguating if necessary.
        /// </summary>
        /// <param name="listName">
        /// The name of the list to find.
        /// </param>
        /// <param name="requireExact">
        /// <c>true</c> if an exact match is required; otherwise <c>false</c> to allow disambiguation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the list if found; otherwise <see langword = "null" />.
        /// </returns>
        private async Task<TodoList> FindListAsync(string listName, bool requireExact)
        {
            // Get the service
            var service = TodoService.GetDefault();

            // Load all lists
            var lists = await service.LoadListsAsync();

            // Try to find a list that matches exactly
            var list = lists.Where(l => l.Title.Equals(listName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            // If no exact match was found and it's allowed, disambiguate
            if ((list == null) && (!requireExact))
            {
                var closeMatches = lists.Where(l => Math.Abs(l.Title.CompareTo(listName)) < CloseMatchOffset);
                if (closeMatches.Count() > 1)
                {
                    // Attempt to disambiguate
                    list = await Disambiguate(
                        closeMatches, // Which items?
                        (l) => l.Title, // Title
                        (l) => string.Format("{0} item{1}", l.Items.Count, l.Items.Count != 1 ? "s" : ""), // Description
                        "Which list did you mean?", // Message 1
                        "I'm sorry, which list?" // Message 2
                        );
                }
                else
                {
                    // Only one or no lists are close matches
                    list = closeMatches.FirstOrDefault();
                }
            }

            // Return whatever we've found
            return list;
        }

        /// <summary>
        /// Handle a request to add an item to a list. This task demonstrates how to prompt a user
        /// for confirmation of an operation and showing a completion screen.
        /// </summary>
        /// <param name="listName">
        /// The name of the list.
        /// </param>
        /// <param name="itemName">
        /// The name of the item.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        private async Task HandleAddToListAsync(string listName, string itemName)
        {
            // Load all lists. If this operation could take a long time (e.g. requiring 
            // a response from a remote web service) consider inserting a progress screen 
            // here to prevent Cortana from timing out. 
            await ReportProgressAsync(string.Format("Looking for the {0} list", listName));

            // Find the list
            var list = await FindListAsync(listName, false);

            // If the list couldn't be found, notify the user and bail
            if (list == null)
            {
                await ReportCompleteAsync("I'm sorry, a list couldn't be found with that name.");
                return;
            }

            // Add the item to the list
            list.Items.Add(new TodoItem()
            {
                Title = itemName
            });

            // Save the list
            await TodoService.GetDefault().SaveAsync(list);

            // Notify complete
            await ReportCompleteAsync(string.Format("Okay, I've added {0} to your {1} list.", itemName, list.Title));
        }

        /// <summary>
        /// Handles a request to mark an item complete in a list. This task demonstrates how to prompt a user
        /// for confirmation of an operation and showing a completion screen.
        /// </summary>
        /// <param name="listName">
        /// The name of the list.
        /// </param>
        /// <param name="itemName">
        /// The name of the item.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        private async Task HandleMarkItemCompleteAsync(string listName, string itemName)
        {
            await ReportProgressAsync(string.Format("Looking for {0} in the {1} list", itemName, listName));

            // Find the list
            var list = await FindListAsync(listName, false);

            // If the list couldn't be found, notify the user and bail
            if (list == null)
            {
                await ReportCompleteAsync("I'm sorry, a list couldn't be found with that name.");
                return;
            }

            // Find the item
            var item = await FindItemAsync(list, itemName, false);

            // If the item couldn't be found, notify the user and bail
            if (item == null)
            {
                await ReportCompleteAsync("I'm sorry, an item couldn't be found with that name.");
                return;
            }

            // Mark the item complete
            item.IsComplete = true;

            // Save the list
            await TodoService.GetDefault().SaveAsync(list);

            // Notify complete
            await ReportCompleteAsync(string.Format("Okay, {0} completed.", itemName));
        }

        /// <summary>
        /// Handle a request to create a new list with the specified name.
        /// </summary>
        /// <param name="listName">
        /// The name of the list.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        private async Task HandleNewListAsync(string listName)
        {
            // Load all lists. If this operation could take a long time (e.g. requiring 
            // a response from a remote web service) consider inserting a progress screen 
            // here to prevent Cortana from timing out. 
            await ReportProgressAsync(string.Format("Creating the {0} list", listName));

            // Look for existing list
            var list = await FindListAsync(listName, true);

            // If the list already exists, notify the user and bail
            if (list == null)
            {
                await ReportCompleteAsync(string.Format("I'm sorry, there's already a list called {0}.", listName));
                return;
            }

            // Create the new list
            list = new TodoList()
            {
                Title = listName
            };

            // Save the list
            await TodoService.GetDefault().SaveAsync(list);

            // Notify complete
            await ReportCompleteAsync(string.Format("Sure, a {0} list was created.", listName));
        }

        /// <summary>
        /// Reports progress. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        private async Task ReportProgressAsync(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceConnection.ReportProgressAsync(response);
        }

        /// <summary>
        /// Shows a completion message to the user.
        /// </summary>
        /// <param name="message">
        /// The message to display (and speak).
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        private async Task ReportCompleteAsync(string message)
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = userMessage.SpokenMessage = message;

            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceConnection.ReportSuccessAsync(response);
        }

        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        private async Task RequestAppLaunchAsync()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launching Cortana Todo";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceConnection.RequestAppLaunchAsync(response);
        }
        #endregion // Internal Methods

        /// <summary>
        /// Background task entrypoint. Voice Commands using the <VoiceCommandService Target="...">
        /// tag will invoke this when they are recognized by Cortana, passing along details of the 
        /// invocation. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/en-us/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// Cortana dismisses its UI if it loses focus. This will cause it to terminate the background
        /// task, even if the background task is being debugged. Use of Remote Debugging is recommended
        /// in order to debug background task behaviors. In order to debug background tasks, open the
        /// project properties for the app package (not the background task project), and enable
        /// Debug -> "Do not launch, but debug my code when it starts". Alternatively, add a long
        /// initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();

            // Register to receive an event if Cortana dismisses the background task. This will
            // occur if the task takes too long to respond, or if Cortana's UI is dismissed.
            // Any pending operations should be cancelled or waited on to clean up where possible.
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            // This should match the uap:AppService and VoiceCommandService references from the 
            // package manifest and VCD files, respectively. Make sure we've been launched by
            // a Cortana Voice Command.
            if (triggerDetails != null && triggerDetails.Name == "CortanaTodoVoiceService")
            {
                try
                {
                    // Get the connection
                    voiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    // Subscribe to commands completed
                    voiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                    // Get the command
                    VoiceCommand voiceCommand = await voiceConnection.GetVoiceCommandAsync();

                    Debug.WriteLine(string.Format("Command Name: {0}", voiceCommand.CommandName));
                    Debug.WriteLine(string.Format("Result.Text: {0}", voiceCommand.SpeechRecognitionResult.Text));
                    foreach (var rule in voiceCommand.SpeechRecognitionResult.RulePath)
                    {
                        Debug.WriteLine(string.Format("Rule: {0}", rule));
                    }
                    foreach (var prop in voiceCommand.Properties)
                    {
                        foreach (var value in prop.Value)
                        {
                            Debug.WriteLine(string.Format("Prop: {0} Val: {1}", prop.Key, value));
                        }
                    }
                    foreach (var prop in voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties)
                    {
                        foreach (var value in prop.Value)
                        {
                            Debug.WriteLine(string.Format("S-Prop: {0} Val: {1}", prop.Key, value));
                        }
                    }
                    
                    // Depending on the operation (defined in VoiceCommands.xml)
                    // perform the appropriate command.
                    string listName = null;
                    string itemName = null;
                    switch (voiceCommand.CommandName)
                    {
                        case "addToList":
                            listName = voiceCommand.Properties["listName"].FirstOrDefault();
                            itemName = voiceCommand.Properties["itemName"].FirstOrDefault();
                            await HandleAddToListAsync(listName, itemName);
                            break;

                        case "addNewList":
                            listName = voiceCommand.Properties["listNameNL"].FirstOrDefault();
                            await HandleNewListAsync(listName);
                            break;

                        case "markItemComplete":
                            listName = voiceCommand.Properties["listName"].FirstOrDefault();
                            itemName = voiceCommand.Properties["itemName"].FirstOrDefault();
                            await HandleMarkItemCompleteAsync(listName, itemName);
                            break;
                            
                        default:
                            // As with app activation VCDs, we need to handle the possibility that
                            // an app update may remove a voice command that is still registered.
                            // This can happen if the user hasn't run an app since an update.
                            await RequestAppLaunchAsync();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }



        /// <summary>
        /// Handle the completion of the voice command. Your app may be cancelled
        /// for a variety of reasons, such as user cancellation or not providing 
        /// progress to Cortana in a timely fashion. Clean up any pending long-running
        /// operations (eg, network requests).
        /// </summary>
        /// <param name="sender">The voice connection associated with the command.</param>
        /// <param name="args">Contains an Enumeration indicating why the command was terminated.</param>
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}

