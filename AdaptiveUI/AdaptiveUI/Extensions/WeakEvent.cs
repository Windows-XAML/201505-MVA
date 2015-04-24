using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveUI.Extensions
{
    /// <summary>
    /// Allows multiple subscriptions to an event without pinning subscribers in memory.
    /// </summary>
    /// <typeparam name="THandler">
    /// The type of delegate the represents the signature of the event.
    /// </typeparam>
    public class WeakEvent<THandler> : WeakEvent where THandler : class
    {

        #region Nested Types
        private class WeakSubscription
        {
            public WeakSubscription(Delegate del)
            {
                Instance = new WeakReference(del.Target);
                Method = del.GetMethodInfo();
            }
            public WeakReference Instance { get; set; }
            public MethodInfo Method { get; set; }
        }

        private class SubscriberCollection : Collection<WeakSubscription> { }
        #endregion // Nested Types

        #region Member Variables
        private SubscriberCollection subscribers = new SubscriberCollection();
        #endregion // Member Variables


        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="WeakEvent"/> instance.
        /// </summary>
        /// <param name="source">
        /// The source object that will be raising the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event to subscribe to.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal WeakEvent(object source, string eventName) : base(source, eventName)
        {
            // Verify handler type
            if (!typeof(Delegate).GetTypeInfo().IsAssignableFrom(typeof(THandler).GetTypeInfo()))
            {
                throw new InvalidOperationException(string.Format("The type '{0}' is not a delegate and cannot be used for event subscriptions.", typeof(THandler).Name));
            }
        }
        #endregion // Constructors

        #region Overrides / Event Handlers
        protected override void OnEventRaised(params object[] parameters)
        {
            // Pass to base first
            base.OnEventRaised(parameters);

            // Do for all subscribers
            for (int iSub = subscribers.Count - 1; iSub >= 0; iSub--)
            {
                // Get the subscription
                var sub = subscribers[iSub];

                // Alive?
                if (sub.Instance.IsAlive)
                {
                    // Call the subscriber safely
                    try
                    {
                        sub.Method.Invoke(sub.Instance.Target, parameters);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("WARNING: Unsubscribing handler due to failure: {0}", ex.Message));
                        subscribers.RemoveAt(iSub);
                    }
                }
                else
                {
                    
                    // Reference dead. Might as well remove it.
                    subscribers.RemoveAt(iSub);
                }
            }
        }
        #endregion // Overrides / Event Handlers

        #region Public Methods
        /// <summary>
        /// Adds a weak subscription to the specified handler.
        /// </summary>
        /// <param name="handler">
        /// The handler to subscribe.
        /// </param>
        public void Subscribe(THandler handler)
        {
            // Cast to delegate
            var del = handler as Delegate;

            // Get the target
            var target = del.Target;

            // Get the new info
            var method = del.GetMethodInfo();

            // Make sure it's not already subscribed
            for (int iSub = subscribers.Count - 1; iSub >= 0; iSub--)
            {
                // Get the subscription
                var sub = subscribers[iSub];

                // Subscriber still alive?
                if (sub.Instance.IsAlive)
                {
                    // Already subscribed?
                    if ((sub.Instance.Target == target) && (sub.Method == method)) { return; }
                }
                else
                {
                    // Reference dead. Might as well remove it.
                    subscribers.RemoveAt(iSub);
                }
            }

            // Not subscribed yet. Subscribe.
            subscribers.Add(new WeakSubscription(del));
        }

        /// <summary>
        /// Removes a subscription to the specified handler.
        /// </summary>
        /// <param name="handler">
        /// The handler to unsubscribe.
        /// </param>
        public void Unsubscribe(THandler handler)
        {
            // Cast to delegate
            var del = handler as Delegate;

            // Get the target
            var target = del.Target;

            // Get the method info
            var method = del.GetMethodInfo();

            // See if found
            for (int iSub = subscribers.Count - 1; iSub >= 0; iSub--)
            {
                // Get the subscription
                var sub = subscribers[iSub];

                // Still alive?
                if (sub.Instance.IsAlive)
                {
                    // Match?
                    if ((sub.Instance.Target == target) && (sub.Method == method))
                    {
                        subscribers.RemoveAt(iSub);
                        return;
                    }
                }
                else
                {
                    // Reference dead. Might as well remove it.
                    subscribers.RemoveAt(iSub);
                }
            }
        }
        #endregion // Public Methods
    }

    public class WeakEvent : EventWatcher
    {
        #region Static Version
        #region Member Variables
        static private Collection<WeakEvent> registrations = new Collection<WeakEvent>();
        #endregion // Member Variables


        #region Internal Methods
        static private WeakEvent<THandler> FindRegistration<THandler>(object source, string eventName) where THandler : class
        {
            // Look for registration
            WeakEvent<THandler> reg = null;
            for (int iReg = registrations.Count - 1; iReg >= 0; iReg--)
            {
                // Get the registration
                reg = registrations[iReg] as WeakEvent<THandler>;

                // If the source is dead, remove the registration
                if ((reg != null) && (!reg.source.IsAlive))
                {
                    registrations.RemoveAt(iReg);
                    reg = null;
                }

                // If it's the right source, done searching
                if (reg.source.Target == source)
                {
                    break;
                }
                else
                {
                    reg = null;
                }
            }

            // Done searching
            return reg;
        }
        #endregion // Internal Methods


        #region Public Methods
        /// <summary>
        /// Adds a weak subscription to the specified handler.
        /// </summary>
        /// <param name="source">
        /// The object that is the source of the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event.
        /// </param>
        /// <param name="handler">
        /// The handler to subscribe.
        /// </param>
        static public void Subscribe<THandler>(object source, string eventName, THandler handler) where THandler : class
        {
            // Try to find existing
            var reg = FindRegistration<THandler>(source, eventName);

            // If not found, create one and store it
            if (reg == null)
            {
                reg = new WeakEvent<THandler>(source, eventName);
                registrations.Add(reg);
            }

            // Add the subscription
            reg.Subscribe(handler);
        }

        /// <summary>
        /// Removes a subscription to the specified handler.
        /// </summary>
        /// <param name="source">
        /// The object that is the source of the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event.
        /// </param>
        /// <param name="handler">
        /// The handler to unsubscribe.
        /// </param>
        static public void Unsubscribe<THandler>(object source, string eventName, THandler handler) where THandler : class
        {
            // Try to find existing
            var reg = FindRegistration<THandler>(source, eventName);

            // If found, unsubscribe
            if (reg == null)
            {
                reg.Subscribe(handler);
            }
        }
        #endregion // Public Methods
        #endregion // Static Version


        #region Instance Version
        #region Member Variables
        private WeakReference source;
        private string eventName;
        #endregion // Member Variables


        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="WeakEvent"/> instance.
        /// </summary>
        /// <param name="source">
        /// The source object that will be raising the event.
        /// </param>
        /// <param name="eventName">
        /// The name of the event to subscribe to.
        /// </param>
        protected WeakEvent(object source, string eventName) : base(source, eventName)
        {
            // Store
            this.source = new WeakReference(source);
            this.eventName = eventName;
        }
        #endregion // Constructors
        #endregion // Instance Version
    }
}
