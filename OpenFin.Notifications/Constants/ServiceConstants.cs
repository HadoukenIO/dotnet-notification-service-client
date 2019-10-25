using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFin.Notifications.Constants
{
    internal static class ServiceConstants
    {
        internal static string NotificationServiceChannelName = "of-notifications-service-v1";
#if DEBUG
        internal static string NotificationsServiceManifestUrl = "http://localhost:3922/provider/app.json";
#else
        internal static string NotificationsServiceManifestUrl = "https://cdn.openfin.co/services/openfin/notifications/app.json";
#endif
    }
}
