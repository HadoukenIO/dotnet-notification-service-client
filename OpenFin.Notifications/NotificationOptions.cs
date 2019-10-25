using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenFin.Notifications
{
    public class NotificationOptions
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("customData")]
        public Dictionary<string, object> CustomData { get; set; }

        [JsonProperty("onSelect")]
        public Dictionary<string, object> OnNotificationSelect { get; set; }

        [JsonProperty("onExpire")]
        public Dictionary<string, object> OnNotificationExpired { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [JsonProperty("buttons")]
        public IEnumerable<ButtonOptions> Buttons { get; set; }

        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
    }
}