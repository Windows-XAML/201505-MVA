using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Template10.Services
{
    /// <summary>
    /// Represents options for running tasks.
    /// </summary>
    public class TaskRunOptions
    {
        #region Static Version
        static private TaskRunOptions defaultInstance;
        #region Public Methods
        /// <summary>
        /// Creates a <see cref="TaskRunOptions"/> instance with the specified failure message.
        /// </summary>
        /// <param name="failureMessage">
        /// The failure message to use.
        /// </param>
        /// <returns>
        /// A new <see cref="TaskRunOptions"/> instance.
        /// </returns>
        static public TaskRunOptions WithFailure(string failureMessage)
        {
            return new TaskRunOptions() { FailureMessge = failureMessage };
        }
        #endregion // Public Methods

        /// <summary>
        /// Gets the default <see cref="TaskRunOptions"/>.
        /// </summary>
        static public TaskRunOptions Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new TaskRunOptions();
                }
                return defaultInstance;
            }
        }
        #endregion // Static Version

        #region Instance Version
        #region Public Properties
        /// <summary>
        /// <c>true</c> if the failure messages should be displayed to the user if the task fails; otherwise <c>false</c>. The default is <c>true</c>.
        /// </summary>
        public bool DisplayFailures { get; set; } = true;

        /// <summary>
        /// <c>true</c> if exception information should be displayed to the user if the task fails; otherwise <c>false</c>. The default is <c>true</c>.
        /// </summary>
        public bool DisplayExceptionInfo { get; set; } = true;

        /// <summary>
        /// Gets or sets a message to display if the operation fails e.g. "There was a problem saving documents" or "Unable to load user data".
        /// </summary>
        public string FailureMessge { get; set; } = "Oops, there was a problem.";

        /// <summary>
        /// <c>true</c> if the caller should be considered "busy" while the task is running; otherwise <c>false</c>. The default is <c>true</c>.
        /// </summary>
        public bool IsBusy { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="TaskScheduler"/> used to execute the task or action.
        /// </summary>
        /// <remarks>
        /// If this property is <see langword="null"/> tasks will execute using their defult scheduler and actions will run using 
        /// <see cref="TaskScheduler.FromCurrentSynchronizationContext"/>.
        /// </remarks>
        public TaskScheduler Scheduler { get; set; }
        #endregion // Public Properties
        #endregion // Instance Version
    }

    /// <summary>
    /// A helper class for working with tasks.
    /// </summary>
    static public class TaskHelper
    {
        #region Member Variables
        static Task completedTask;
        #endregion // Member Variables


        #region Internal Methods
        /// <summary>
        /// Display information about the error if error display is turned on.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <param name="options">
        /// The <see cref="TaskRunOptions"/> that controls how errors are displayed.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        static private async Task DisplayErrorAsync(Exception ex, TaskRunOptions options)
        {
            // Show failures?
            if (options.DisplayFailures)
            {
                // Build failure message
                string message;
                if (options.DisplayExceptionInfo)
                {
                    message = string.Format("{0} \r\n\r\n{1}", options.FailureMessge, ex.Message);
                }
                else
                {
                    message = options.FailureMessge;
                }

                // Show failure message
                await new MessageDialog(message).ShowAsync();
            }
        }
        #endregion // Internal Methods


        #region Public Methods
        /// <summary>
        /// Executes a task and handles any errors that occur.
        /// </summary>
        /// <param name="taskFunction">
        /// A function that yields the <see cref="Task"/> to execute.
        /// </param>
        /// <param name="options">
        /// A <see cref="TaskRunOptions"/> instance that specifies options for running the task or <see langword="null"/> 
        /// to use <see cref="TaskRunOptions.Default"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the wrapped execution of the inner task.
        /// </returns>
        static public async Task RunWithErrorHandling(Func<Task> taskFunction, TaskRunOptions options = null)
        {
            // Validate
            if (taskFunction == null) throw new ArgumentNullException("taskFunction");

            // Options
            if (options == null) { options = TaskRunOptions.Default; }

            // Handle failure
            try
            {
                // Custom scheduler?
                if (options.Scheduler != null)
                {
                    await new TaskFactory(options.Scheduler).StartNew(taskFunction).Unwrap();
                }
                else
                {
                    await taskFunction();
                }
            }
            catch (Exception ex)
            {
                await DisplayErrorAsync(ex, options);
            }
        }

        /// <summary>
        /// Executes a task and handles any errors that occur.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to execute.
        /// </param>
        /// <param name="options">
        /// A <see cref="TaskRunOptions"/> instance that specifies options for running the task or <see langword="null"/> 
        /// to use <see cref="TaskRunOptions.Default"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the wrapped execution of the inner action.
        /// </returns>
        static public async Task RunWithErrorHandling(Action action, TaskRunOptions options = null)
        {
            // Validate
            if (action == null) throw new ArgumentNullException("action");

            // Options
            if (options == null) { options = TaskRunOptions.Default; }

            // Handle failure
            try
            {
                // Custom scheduler
                if (options.Scheduler != null)
                {
                    await new TaskFactory(options.Scheduler).StartNew(action);
                }
                else
                {
                    await Task.Factory.StartNew(action);
                }
            }
            catch (Exception ex)
            {
                await DisplayErrorAsync(ex, options);
            }
        }
        #endregion // Public Methods


        #region Public Properties
        /// <summary>
        /// Gets a task that can be returned to specify that a synchronous function has completed.
        /// </summary>
        static public Task CompletedTask
        {
            get
            {
                if (completedTask == null)
                {
                    completedTask = Task.FromResult(true);
                }
                return completedTask;
            }
        }
        #endregion // Public Properties
    }
}
