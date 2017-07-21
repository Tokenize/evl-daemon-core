using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EvlDaemon.Events.Notifiers
{
    public class EmailNotifier : IEventNotifier
    {
        public string Name { get; private set; }
        public Command.PriorityLevel PriorityLevel { get; private set; }
        public string Sender { get; private set; }
        public string Recipient { get; private set; }

        private string apiKey;

        public EmailNotifier(string name, Command.PriorityLevel priorityLevel, string apiKey,
            string sender, string recipient)
        {
            Name = name;
            PriorityLevel = priorityLevel;
            Sender = sender;
            Recipient = recipient;

            this.apiKey = apiKey;
        }

        public async Task NotifyAsync(Event e)
        {
            try
            {
                if (e.Priority >= PriorityLevel)
                {
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(Sender);
                    var to = new EmailAddress(Recipient);

                    string textMessage = $"[{e.Timestamp.ToString()}]: {e.Description}";
                    string subject = $"EVL event notification at {e.Timestamp.ToString("o")}";;

                    var msg = MailHelper.CreateSingleEmail(from, to, subject, textMessage, null);

                    await client.SendEmailAsync(msg);
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