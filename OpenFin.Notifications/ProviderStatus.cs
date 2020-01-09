using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFin.Notifications
{
    /// <summary>
    /// Provider status model.
    /// </summary>
    public class ProviderStatus
    {
        /// <summary>
        /// Determines if the client is connnected to the service.
        /// </summary>
        [JsonProperty("connected")]
        public bool Connected { get; set; }

        /// <summary>
        /// The version of the service provider the client is connected to.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
