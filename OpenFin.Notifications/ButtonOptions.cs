using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenFin.Notifications
{
    /// <summary>
    /// Configuraton options for constructing a button within a notification.
    /// </summary>
    public sealed class ButtonOptions
    {
        /// <summary>
        /// Identifies the type of this control.
        /// </summary>
        [JsonProperty("type")]
        public string Type = "button";

        /// <summary>
        /// The button text.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The url of the icon to be placed to the left of the button text.
        /// </summary>
        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        /// <summary>
        /// Defines the data to be passed back to the application via a notification-action event when a button is clicked.
        /// </summary>
        [JsonProperty("onClick")]
        public Dictionary<string, object> OnNotificationButtonClick { get; set; }
    }
}