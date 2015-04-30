using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Services;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

#if MVVMLIGHT
using VMBase = GalaSoft.MvvmLight.ViewModelBase;
#else
using VMBase = Template10.Mvvm.ViewModelBase;
#endif

namespace Template10.Mvvm
{
    /// <summary>
    /// A base class for ViewModels
    /// </summary>
    public abstract class ViewModel : VMBase, INavigatable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="ViewModel"/> instance.
        /// </summary>
        public ViewModel()
        {
            // Call initialize using Fire and Forget
            var t = InitializeAsync();
        }
        #endregion // Constructors

        #region Internal Methods
        /// <summary>
        /// Creates the commands for the <see cref="ViewModel"/> and stores them in the <see cref="Commands"/> collection.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method uses the <see cref="CommandHelper"/> class and command related attributes.
        /// </remarks>
        protected virtual void CreateCommands()
        {
            // Default implementation uses the CommandHelper and the command attributes
            Commands = CommandHelper.CreateCommands(this);
        }

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
        protected async Task RunWithErrorHandling(Func<Task> taskFunction, TaskRunOptions options = null)
        {
            // Busy?
            if (options.IsBusy)
            {
                isBusy = true;
            }

            // Use task helper
            await TaskHelper.RunWithErrorHandling(taskFunction, options);

            // No longer busy?
            if (options.IsBusy)
            {
                isBusy = false;
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
        protected async Task RunWithErrorHandling(Action action, TaskRunOptions options = null)
        {
            // Busy?
            if (options.IsBusy)
            {
                isBusy = true;
            }

            // Use task helper
            await TaskHelper.RunWithErrorHandling(action, options);

            // No longer busy?
            if (options.IsBusy)
            {
                isBusy = false;
            }
        }

        /// <summary>
        /// Allows inherited classes to change the initialization strategy for the ViewMode.
        /// </summary>
        /// <remarks>
        /// <see cref="InitializeAsync"/> is called from the <see cref="ViewModel"/> base class constructor. The default 
        /// initialization strategy is to call <see cref="LoadDesignData"/> if <see cref="IsInDesignMode"/> is <c>true</c> 
        /// or call <see cref="ObtainServices"/>, <see cref="CreateCommands"/> and <see cref="LoadDefaultDataAsync"/> if 
        /// <see cref="IsInDesignMode"/> is <c>false</c>.
        /// </remarks>
        protected virtual async Task InitializeAsync()
        {
            // Design mode or regular?
            if (IsInDesignMode)
            {
                LoadDesignData();
            }
            else
            {
                ObtainServices();
                CreateCommands();
                await LoadDefaultDataAsync();
            }
        }

        /// <summary>
        /// Loads optional default data for the ViewModel.
        /// </summary>
        /// <remarks>
        /// This method is called as part of the default <see cref="InitializeAsync"/> strategy when 
        /// <see cref="IsInDesignMode"/> is <c>false</c>.
        /// </remarks>
        protected virtual Task LoadDefaultDataAsync()
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Loads optional design-time data for the ViewModel.
        /// </summary>
        /// <remarks>
        /// This method is called as part of the default <see cref="InitializeAsync"/> strategy when 
        /// <see cref="IsInDesignMode"/> is <c>true</c>.
        /// </remarks>
        protected virtual void LoadDesignData()
        {

        }

        /// <summary>
        /// Obtains any services needed by the ViewMode.
        /// </summary>
        /// <remarks>
        /// This method is called as part of the default <see cref="InitializeAsync"/> strategy when 
        /// <see cref="IsInDesignMode"/> is <c>false</c>.
        /// </remarks>
        protected virtual void ObtainServices()
        {

        }
        #endregion // Internal Methods

        #region Overridables / Event Triggers
        /// <summary>
        /// Allows inherited classes to handle navigation to the ViewModel (and page).
        /// </summary>
        /// <param name="parameter">
        /// The parameter used as part of navigation.
        /// </param>
        /// <param name="mode">
        /// The navigation mode (New, Back, Forward, etc.)
        /// </param>
        /// <param name="state">
        /// Any persisted state for the ViewModel.
        /// </param>
        protected virtual void OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
        }

        /// <summary>
        /// Allows inherited classes to handle navigation from the ViewModel (and page).
        /// </summary>
        /// <param name="parameter">
        /// The parameter used as part of navigation.
        /// </param>
        /// <param name="mode">
        /// The navigation mode (New, Back, Forward, etc.)
        /// </param>
        /// <param name="state">
        /// Any persisted state for the ViewModel.
        /// </param>
        protected virtual void OnNavigatedFrom(Dictionary<string, object> state, bool suspending)
        {
        }
        #endregion // Overridables / Event Triggers


        #region INavigatable Interface
        void INavigatable.OnNavigatedTo(string parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            this.OnNavigatedTo(parameter, mode, state);
        }

        void INavigatable.OnNavigatedFrom(Dictionary<string, object> state, bool suspending)
        {
            this.OnNavigatedFrom(state, suspending);
        }
        #endregion // INavigatable Interface


        #region Public Properties
        private CommandCollection commands = new CommandCollection();
        /// <summary>
        /// Gets or sets the collection of commands for the <see cref="ViewModel"/>.
        /// </summary>
        /// <value>
        /// The collection of commands for the <see cref="ViewModel"/>.
        /// </value>
        public CommandCollection Commands
        {
            get
            {
                return commands;
            }
            set
            {
                Set(ref commands, value);
            }
        }

        bool isBusy = false;
        /// <summary>
        /// Gets or sets a value that indiciates if the <see cref="ViewMode"/> is busy performing work.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            protected set
            {
                Set(ref isBusy, value);
            }
        }

        #if !MVVMLIGHT
        /// <summary>
        /// Gets a value that indicates if the <see cref="ViewModel"/> is currently being used at design time.
        /// </summary>
        /// <remarks>
        /// The default implementation simply returns Windows.ApplicationModel.DesignMode.DesignModeEnabled.
        /// </remarks>
        public virtual bool IsInDesignMode
        {
            get
            {
                return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
            }
        }
        #endif
        #endregion // Public Properties
    }
}