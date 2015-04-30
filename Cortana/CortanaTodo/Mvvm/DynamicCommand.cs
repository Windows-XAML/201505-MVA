using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace Template10.Mvvm
{
    /// <summary>
    /// Represents a dynamically generated command.
    /// </summary>
    /// <remarks>
    /// Instances of this class are generally created by the <see cref="CommandHelper"/> class using 
    /// <see cref="CommandAttribute"/>, <see cref="CommandCanExecuteAttribute"/> and 
    /// <see cref="CommandCanExecuteChangedAttribute"/>.
    /// </remarks>
    public class DynamicCommand : INamedCommand
    {
        #region Static Version
        #region Internal Methods
        /// <summary>
        /// Validates a <see cref="MethodInfo"/> for use as the CanExecute handler for an ICommand.
        /// </summary>
        /// <param name="executeMethod">
        /// The <see cref="MethodInfo"/> to validate.
        /// </param>
        static private void ValidateCanExecuteMethod(MethodInfo canExecuteMethod)
        {
            // Validate return type
            if (canExecuteMethod.ReturnType != typeof(bool))
            {
                throw new InvalidOperationException(string.Format("The method {0} must return a bool", canExecuteMethod.Name));
            }

            // Validate parameters
            ValidateExecuteMetod(canExecuteMethod);
        }

        /// <summary>
        /// Validates a <see cref="MethodInfo"/> for use as the Execute handler for an ICommand.
        /// </summary>
        /// <param name="executeMethod">
        /// The <see cref="MethodInfo"/> to validate.
        /// </param>
        static private void ValidateExecuteMetod(MethodInfo executeMethod)
        {
            // Get params
            var par = executeMethod.GetParameters();

            if (par.Length > 1)
            {
                throw new InvalidOperationException(string.Format("The method {0} must accept either 0 or 1 argument(s)", executeMethod.Name));
            }
        }
        #endregion // Internal Methods
        #endregion // Static Version


        #region Instance Version
        #region Member Variables
        private Collection<string> canExecuteProperties;
        private bool changeSubscribed = false;
        private object commandSource;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="DynamicCommand"/> instance.
        /// </summary>
        /// <param name="commandSource">
        /// The object that is the source of the command.
        /// </param>
        public DynamicCommand(object commandSource, string name)
        {
            // Validate
            if (commandSource == null) throw new ArgumentNullException("commandSource");
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

            // Store
            this.commandSource = commandSource;
            Name = name;
        }
        #endregion // Constructors

        #region Overrides / Event Handlers
        private void CommandSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the property name is in the list, raise the event
            if (canExecuteProperties.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }
        #endregion // Overrides / Event Handlers

        #region Public Methods
        /// <summary>
        /// Adds the named property as a property that should trigger the CanExecuteChanged event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        public void AddCanExecuteProperty(string propertyName)
        {
            // Validate
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("propertyName");

            // Create collection if it doesn't exist
            if (canExecuteProperties == null)
            {
                canExecuteProperties = new Collection<string>();
            }

            // If not in the list, add it
            if (!canExecuteProperties.Contains(propertyName))
            {
                canExecuteProperties.Add(propertyName);
            }

            // If not subscribed, subscribe
            if (!changeSubscribed)
            {
                // Get as INotifyPropertyChanged
                var iNotifySource = commandSource as INotifyPropertyChanged;

                // Make sure it implements the interface
                if (iNotifySource == null)
                {
                    var msg = string.Format("An attempt was made to add the property '{0}' as a trigger for a command but the type '{1}' does not implement {2}.", propertyName, commandSource.GetType().Name, nameof(INotifyPropertyChanged));
                    throw new InvalidOperationException(msg);
                }

                // Mark as subscribed
                changeSubscribed = true;

                // Subscribe
                iNotifySource.PropertyChanged += CommandSource_PropertyChanged;
            }
        }

        /// <summary>
        /// Evaluates whether or not the command can be executed with the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter for the command.
        /// </param>
        /// <returns>
        /// <c>true</c> if the command can be executed with the specified parameter; otherwise <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod == null)
            {
                return true;
            }

            if (canExecuteMethod.GetParameters().Length == 1)
            {
                return (bool)canExecuteMethod.Invoke(commandSource, new object[] { parameter });
            }
            else
            {
                return (bool)canExecuteMethod.Invoke(commandSource, null);
            }
        }

        /// <summary>
        /// Executes the command with the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to execute the command with.
        /// </param>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter) && (executeMethod != null))
            {
                if (canExecuteMethod.GetParameters().Length == 1)
                {
                    executeMethod.Invoke(commandSource, new object[] { parameter });
                }
                else
                {
                    executeMethod.Invoke(commandSource, null);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Removes the named property as a property that should trigger the CanExecuteChanged event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        public void RemoveCanExecuteProperty(string propertyName)
        {
            // Validate
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("propertyName");

            // Remove if active
            if ((canExecuteProperties != null) && (canExecuteProperties.Contains(propertyName)))
            {
                // Remove
                canExecuteProperties.Remove(propertyName);

                // If all properties have been removed, unsubscribe
                if ((canExecuteProperties.Count == 0) && (changeSubscribed))
                {
                    // Get as INotifyPropertyChanged
                    var iNotifySource = commandSource as INotifyPropertyChanged;

                    // Unsubscribe
                    iNotifySource.PropertyChanged -= CommandSource_PropertyChanged;

                    // Mark as unsubscribed
                    changeSubscribed = false;
                }
            }
        }
        #endregion // Public Methods


        #region Public Properties
        private MethodInfo canExecuteMethod;
        /// <summary>
        /// Gets or sets the function that evaluates whether or not the command can be executed.
        /// </summary>
        /// <value>
        /// The function that evaluates whether or not the command can be executed.
        /// </value>
        public MethodInfo CanExecuteMethod
        {
            get
            {
                return canExecuteMethod;
            }
            set
            {
                // Make sure changing
                if (canExecuteMethod != value)
                {
                    // Validate
                    if (value != null)
                    {
                        ValidateCanExecuteMethod(value);
                    }

                    // Store
                    canExecuteMethod = value;

                    // Notify
                    RaiseCanExecuteChanged();
                }
            }
        }

        private MethodInfo executeMethod;
        /// <summary>
        /// Gets or sets the function that handles the execution of the command.
        /// </summary>
        /// <value>
        /// The function that handles the execution of the command.
        /// </value>
        public MethodInfo ExecuteMethod
        {
            get
            {
                return executeMethod;
            }
            set
            {
                // Ensure changing
                if (executeMethod != value)
                {
                    // Validate
                    if (value != null)
                    {
                        ValidateExecuteMetod(value);
                    }

                    // Store
                    executeMethod = value;

                    // Notify
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; private set; }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the value returned from <see cref="CanExecute"/> method may have changed and needs to be reevaluated.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        #endregion // Public Events
        #endregion // Instance Version
    }
}
