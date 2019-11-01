using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenFin.Notifications
{
    /// <summary>
    /// Configuration options for constructing a Notifications object.
    /// </summary>
    public sealed class NotificationOptions
    {
        /// <summary>
        /// The unique identifier for the notification.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The notificaiton body content. Plain text and Markdown (with the exception of links, images, and code blocks) are supported formats.
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// The notification title displayed as the first line of the notification in a heading style.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Describes the context of the notification facilitaing the control of different notification types that are raised by an application.
        /// This property is not displayed on the notification itself
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// The url to the icon to be displayed in the notification.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Any custom context data associated with the notification.
        /// </summary>
        [JsonProperty("customData")]
        public Dictionary<string, object> CustomData { get; set; }

        /// <summary>
        /// Action result to be passed back to the application inside the notification-action event fired when a
        /// notification's body is selected (clicked).
        /// </summary>
        [JsonProperty("onSelect")]
        public Dictionary<string, object> OnNotificationSelect { get; set; }

        /// <summary>
        /// The action result to be returned from the service when
        /// </summary>
        [JsonProperty("onExpire")]
        public Dictionary<string, object> OnNotificationExpired { get; set; }

        /// <summary>
        /// The Timestanp displayed in the notification (default is the current date/time)
        /// </summary>
        [JsonProperty("date")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// A collection of buttons to display in the notification (up to 4 buttons supported).
        /// </summary>
        [JsonProperty("buttons")]
        public IEnumerable<ButtonOptions> Buttons { get; set; }

        /// <summary>
        /// The expiration date and time of the notfication. If not specified, notification will persist until explicity closed.
        /// </summary>
        [JsonProperty("expires")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Expires { get; set; }
    }
}