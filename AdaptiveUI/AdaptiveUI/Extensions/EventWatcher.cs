using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AdaptiveUI.Extensions
{
    /// <summary>
    /// A base class that can monitor any event in a generic way.
    /// </summary>
    public class EventWatcher : IDisposable
    {
        #region Member Variables
        private object source;
        private EventInfo eventInfo;
        private Delegate handler;
        #endregion // Member Variables


        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="EventWatcher"/> instance.
        /// </summary>
        /// <param name="source">
        /// The source object that will be raising the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event to subscribe to.
        /// </param>
        public EventWatcher(object source, string eventName)
        {
            Subscribe(source, eventName);
        }
        #endregion // Constructors


        #region Internal Methods
        /// <summary>
        /// Dynamically creates a handler for any event.
        /// </summary>
        /// <param name="signature">
        /// The <see cref="Type"/> that represents the event (or delegate) signature.
        /// </param>
        private void CreateHandlerDelegate(Type signature)
        {
            // Get the event delegate's parameters from its 'Invoke' method.
            var invokeMethod = signature.GetTypeInfo().GetDeclaredMethod("Invoke");

            // Determine what parameters are passed into the handler
            var parameters = invokeMethod.GetParameters()
                .Select((p) => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            // Create an expression to convert the individual parameters into a param array
            var paramArray = Expression.NewArrayInit(typeof(object), parameters);

            // Get a reference to the EventRaised method
            var eventRaisedMethod = typeof(EventWatcher).GetTypeInfo().GetDeclaredMethod(nameof(OnEventRaised));

            // Create an expression that calls the EventRaised method passing the parameter array
            var body = Expression.Call(Expression.Constant(this), eventRaisedMethod, paramArray);

            // Create a lambda from the body and parameters
            var listener = Expression.Lambda(signature, body, parameters);

            // Compile the lambda, converting it to a delegate
            handler = listener.Compile();
        }

        /// <summary>
        /// Subscribes to the named event on the specified object instance.
        /// </summary>
        /// <param name="source">
        /// The source object that will be raising the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event to subscribe to.
        /// </param>
        private void Subscribe(object source, string eventName)
        {
            // Validate
            if (source == null) throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(eventName)) throw new ArgumentException("eventName");

            // Store
            this.source = source;

            // Attempt to get the runtime event
            eventInfo = source.GetType().GetRuntimeEvent(eventName);

            // Make sure event was found
            if (eventInfo == null)
            {
                throw new InvalidOperationException(string.Format("RuntimeEvent '{0}' was not found on type '{1}'", eventName, source.GetType().Name));
            }

            // Get pointers to the add and remove methods
            MethodInfo addMethod = eventInfo.AddMethod;
            MethodInfo removeMethod = eventInfo.RemoveMethod;

            // Get the parameter list from the add method
            ParameterInfo[] addParameters = addMethod.GetParameters();

            // The method signature for the event handler is the first parameter
            Type delegateType = addParameters[0].ParameterType;

            // Create a generic handler that matches the signature and forwards calls to our EventRaised method
            CreateHandlerDelegate(delegateType);

            // Use the RuntimeMarshaler to subscribe to the event
            Func<object, EventRegistrationToken> add = a => (EventRegistrationToken)addMethod.Invoke(source, new object[] { handler });
            Action<EventRegistrationToken> remove = t => removeMethod.Invoke(source, new object[] { t });
            WindowsRuntimeMarshal.AddEventHandler(add, remove, handler);
        }
        #endregion // Internal Methods


        #region IDisposable Members
        void IDisposable.Dispose()
        {
            // Remove the event handler
            if (handler != null)
            {
                eventInfo.RemoveEventHandler(source, handler);
                handler = null;
            }
        }
        #endregion // IDisposable Members

        #region Overridables / Event Triggers
        /// <summary>
        /// Called when the event being watched is raised.
        /// </summary>
        /// <param name="parameters">
        /// The parameters of the event.
        /// </param>
        protected virtual void OnEventRaised(params object[] parameters)
        {

        }
        #endregion // Overridables / Event Triggers
    }
}
