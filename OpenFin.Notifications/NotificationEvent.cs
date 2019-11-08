using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenFin.Notifications
{
    /// <summary>
    /// The type that defines a notification related event.
    /// </summary>
    public sealed class NotificationEvent
    {
        /// <summary>
        /// The notification configuration data associated with the event that was raised.
        /// </summary>
        [JsonProperty("notification")]
        public NotificationOptions NotificationOptions { get; set; }

        /// <summary>
        /// The type of notification event.
        /// </summary>
        [JsonProperty("type")]
        public string EventType { get; set; }

        /// <summary>
        /// Indicates what action triggered this notification.
        /// </summary>
        [JsonProperty("trigger")]
        public string ActionTrigger { get; set; }

        /// <summary>
        /// The application-defined metadata that is passed back to the application.
        /// </summary>
        [JsonProperty("result")]
        public Dictionary<string, object> NotificationActionResult { get; set; }
    }
}