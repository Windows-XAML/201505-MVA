using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Template10.Services.Lifecycle
{
    public interface ILifecycleAware
    {
        /// <summary>
        /// Allows a class to handle application resume.
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        Task HandleResumeAsync(object e);

        /// <summary>
        /// Allows a class to handle application suspension.
        /// </summary>
        /// <param name="e">
        /// A <see cref="SuspendingEventArgs"/> that contains the event data.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        Task HandleSuspendAsync(SuspendingEventArgs e);
    }
}
