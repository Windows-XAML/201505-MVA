using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Template10.Mvvm
{
    /// <summary>
    /// The interface of a command with a name.
    /// </summary>
    public interface INamedCommand : ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// A collection of commands.
    /// </summary>
    public class CommandCollection : ObservableCollection<ICommand>
    {

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="CommandCollection"/> instance.
        /// </summary>
        public CommandCollection() { }

        /// <summary>
        /// Initializes a new <see cref="CommandCollection"/> with the specified items.
        /// </summary>
        /// <param name="collection"></param>
        public CommandCollection(IEnumerable<ICommand> collection) : base(collection) { }
        #endregion // Constructors


        #region Public Properties
        /// <summary>
        /// Attempts to get the command with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the command to obtain.
        /// </param>
        /// <returns>
        /// The command with the specified name if found; otherwise <see langword = "null" />.
        /// </returns>
        /// <remarks>
        /// This method only works on commands that implement the <see cref="INamedCommand"/> interface.
        /// </remarks>
        public ICommand this[string name]
        {
            get
            {
                // Validate
                if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

                // Try to find
                return this.OfType<INamedCommand>().Where(c => c.Name == name).FirstOrDefault();
            }
        }
        #endregion // Public Properties
    }
}
