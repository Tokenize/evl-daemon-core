using EvlDaemon.Http;
using System;
using System.Threading.Tasks;

namespace EvlDaemon.Events.Notifiers
{
    /// <summary>
    /// A notifier that supports sending SMS messages via the Twilio service.
    /// </summary>
    public class SmsNotifier : IEventNotifier
    {
        public string Name { get; private set; }
        public Command.PriorityLevel PriorityLevel { get; private set; }

        private TwilioHttpClient client;

        public SmsNotifier(string name, Command.PriorityLevel priorityLevel, string url,
            string sid, string authToken, string sender, string recipient)
        {
            Name = name;
            PriorityLevel = priorityLevel;

            client = new TwilioHttpClient(url, recipient, sender, sid, authToken);
        }

        /// <summary>
        /// Sends the given event details to the recipeint via SMS.
        /// </summary>
        /// <param name="e">Event to send</param>
        public async Task NotifyAsync(Event e)
        {
            try
            {
                if (e.Priority >= PriorityLevel)
                {
                    await client.SendSmsAsync(string.Format("[{0}]: {1}", e.Timestamp.ToString("o"),
                        e.Description));
                }
            }
            catch (Exception ex)
            {
                // TODO: Implement logging and log this error.
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
