using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFin.Notifications
{
    public class ButtonOptions
    {
        [JsonProperty("type")]
        public string Type = "button";

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("onClick")]
        public Dictionary<string, object> OnNotificationButtonClick { get; set; }
    }
 
}
