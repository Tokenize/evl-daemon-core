using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon.Http
{
    /// <summary>
    /// A simple HTTP client to be used with the Twilio service. Supports
    /// sending SMS messages.
    /// </summary>
    public class TwilioHttpClient : BaseHttpClient
    {
        public string Recipient { get; set; }
        public string Sender { get; set; }

        private string sid;
        private string authToken;

        public TwilioHttpClient(string baseUrl, string recipient, string sender, string sid, string authToken)
            : base(sid, authToken)
        {
            BaseUrl = baseUrl;
            Recipient = recipient;
            Sender = sender;

            this.sid = sid;
            this.authToken = authToken;
        }

        /// <summary>
        /// Sends the given message to the recipient.
        /// </summary>
        /// <param name="message">Message to send to recipient</param>
        public async Task SendSmsAsync(string message)
        {
            string path = $"/Accounts/{sid}/Messages";
            var data = new[]
            {
                new KeyValuePair<string, string>("To", Recipient),
                new KeyValuePair<string, string>("From", Sender),
                new KeyValuePair<string, string>("Body", message)
            };

            var response = await PostAsync(path, data);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to send SMS message: {response.ReasonPhrase}");
            }
        }
    }
}
