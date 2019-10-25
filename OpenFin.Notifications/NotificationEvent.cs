using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenFin.Notifications
{
    public class NotificationEvent
    {
        [JsonProperty("notification")]
        public NotificationOptions NotificationOptions { get; set; }

        public string Target { get; set; }

        [JsonProperty("type")]
        public string EventType { get; set; }

        [JsonProperty("trigger")]
        public string ActionTrigger { get; set; }

        [JsonProperty("control")]
        public ButtonOptions ControlOptions { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, object> NotificationActionResult { get; set; }
    }
}