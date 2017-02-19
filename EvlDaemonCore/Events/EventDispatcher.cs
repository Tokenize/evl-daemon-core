using EvlDaemon.Events.Notifiers;
using System.Threading.Tasks;

namespace EvlDaemon.Events
{
    /// <summary>
    /// Represents an event dispatcher that dispatches events to all
    /// defined notifiers.
    /// </summary>
    public class EventDispatcher
    {
        private delegate Task EventNotifyHandler(Event e);
        private event EventNotifyHandler Notify;

        /// <summary>
        /// Adds the given <paramref name="notifier"/> to the list of notifiers.
        /// </summary>
        /// <param name="notifier">Notifier to add</param>
        public void AddNotifier(IEventNotifier notifier)
        {
            Notify += notifier.NotifyAsync;
        }

        /// <summary>
        /// Submits the given event <paramref name="e"> to all notifiers.
        /// </summary>
        /// <param name="e">Event to submit</param>
        public void Enqueue(Event e)
        {
            Notify(e);
        }
    }
}
