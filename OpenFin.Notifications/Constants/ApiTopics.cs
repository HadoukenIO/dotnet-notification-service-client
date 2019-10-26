namespace OpenFin.Notifications.Constants
{
    internal static class ApiTopics
    {
        internal const string CreateNotification       = "create-notification";
        internal const string ClearNotification        = "clear-notification";
        internal const string ClearAppNotifications    = "clear-app-notifications";
        internal const string GetAppNotifications      = "fetch-app-notifications";
        internal const string ToggleNotificationCenter = "toggle-notification-center";
        internal const string AddEventListener         = "add-event-listener";
        internal const string RemoveEventListener      = "remove-event-listener";
    }
}