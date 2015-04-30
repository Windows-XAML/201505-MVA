using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Template10.Mvvm
{
    /// <summary>
    /// Base class for command attributes.
    /// </summary>
    public abstract class CommandAttributeBase : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="CommandAttributeBase"/>.
        /// </summary>
        /// <param name="commandName">
        /// The name of the command.
        /// </param>
        public CommandAttributeBase(string commandName)
        {
            // Validate
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentException("commandName");

            // Store
            this.CommandName = commandName;
        }
        #endregion // Constructors

        #region Public Properties
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; private set; }
        #endregion // Public Properties
    }

    /// <summary>
    /// Marks a method as the handler of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : CommandAttributeBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="commandName">
        /// The name of the command.
        /// </param>
        public CommandAttribute(string commandName) : base(commandName) { }
        #endregion // Constructors
    }

    /// <summary>
    /// Marks a method as the evaluator of the availability of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandCanExecuteAttribute : CommandAttributeBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="CommandCanExecuteAttribute"/>.
        /// </summary>
        /// <param name="commandName">
        /// The name of the command.
        /// </param>
        public CommandCanExecuteAttribute(string commandName) : base(commandName) { }
        #endregion // Constructors
    }

    /// <summary>
    /// Marks a property as a trigger that may change the availability of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CommandCanExecuteChangedAttribute : CommandAttributeBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="CommandCanExecuteChangedAttribute"/>.
        /// </summary>
        /// <param name="commandName">
        /// The name of the command.
        /// </param>
        public CommandCanExecuteChangedAttribute(string commandName) : base(commandName) { }
        #endregion // Constructors
    }

    /// <summary>
    /// A class that helps with creating commands for View Models.
    /// </summary>
    static public class CommandHelper
    {
        /// <summary>
        /// Creates a collection of commands for the specified ViewModel.
        /// </summary>
        /// <param name="viewModel">
        /// The ViewModel instance used to determine and create commands.
        /// </param>
        /// <returns>
        /// An observable collection of commands for the View Model.
        /// </returns>
        /// <remarks>
        /// This method leverages <see cref="CommandAttribute"/>, <see cref="CommandCanExecuteAttribute"/> and 
        /// <see cref="CommandCanExecuteChangedAttribute"/> to dynamically build the command collection.
        /// </remarks>
        static public CommandCollection CreateCommands(object viewModel)
        {
            // Validate
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            // Create lookup table
            var lookup = new Dictionary<string, DynamicCommand>();

            // Get type
            var type = viewModel.GetType();

            // Get type info
            var typeInfo = type.GetTypeInfo();

            // Get all methods
            foreach (var method in typeInfo.DeclaredMethods)
            {
                // Check for Execute handlers
                foreach (var executeAttr in method.GetCustomAttributes<CommandAttribute>())
                {
                    // Get or create dynamic command
                    DynamicCommand command = null;
                    if (!lookup.TryGetValue(executeAttr.CommandName, out command))
                    {
                        command = new DynamicCommand(viewModel, executeAttr.CommandName);
                        lookup[executeAttr.CommandName] = command;
                    }

                    // Make sure not already set
                    if (command.ExecuteMethod != null)
                    {
                        throw new InvalidOperationException(string.Format("CommandAttribute applied more than once for command '{0}' on type '{1}'", executeAttr.CommandName, type.Name));
                    }

                    // Set the Execute method
                    command.ExecuteMethod = method;
                }

                // Check for CanExecute handlers
                foreach (var canExecuteAttr in method.GetCustomAttributes<CommandCanExecuteAttribute>())
                {
                    // Get or create dynamic command
                    DynamicCommand command = null;
                    if (!lookup.TryGetValue(canExecuteAttr.CommandName, out command))
                    {
                        command = new DynamicCommand(viewModel, canExecuteAttr.CommandName);
                        lookup[canExecuteAttr.CommandName] = command;
                    }

                    // Make sure not already set
                    if (command.CanExecuteMethod != null)
                    {
                        throw new InvalidOperationException(string.Format("CommandCanExecuteAttribute applied more than once for command '{0}' on type '{1}'", canExecuteAttr.CommandName, type.Name));
                    }

                    // Set the CanExecute method
                    command.CanExecuteMethod = method;
                }
            }

            // Get all properties
            foreach (var prop in typeInfo.DeclaredProperties)
            {
                // Check for Execute handlers
                foreach (var executeChangedAttr in prop.GetCustomAttributes<CommandCanExecuteChangedAttribute>())
                {
                    // Get or create dynamic command
                    DynamicCommand command = null;
                    if (!lookup.TryGetValue(executeChangedAttr.CommandName, out command))
                    {
                        command = new DynamicCommand(viewModel, executeChangedAttr.CommandName);
                        lookup[executeChangedAttr.CommandName] = command;
                    }

                    // Add the property name
                    command.AddCanExecuteProperty(prop.Name);
                }
            }

            // Done looking. Create and return collection.
            return new CommandCollection(lookup.Values);
        }
    }
}
