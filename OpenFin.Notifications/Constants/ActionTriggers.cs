namespace OpenFin.Notifications.Constants
{
    /// <summary>
    /// Constants to use to determine the action trigger provided in a notification action event.
    /// </summary>
    public static class ActionTriggers
    {
        /// <summary>
        /// The value returned when an action was triggered by a control (eg. a button).
        /// </summary>
        public const string Control      = "control";
        /// <summary>
        /// The action raised when a notification's body is clicked.
        /// </summary>
        public const string Select       = "select";

        /// <summary>
        /// The notification was closed, either by interaction, programmatically by an application, or by the notification expiring.
        /// </summary>
        public const string Close        = "close";

        /// <summary>
        /// The notification expired.
        /// </summary>
        public const string Expire       = "expire";

        
        //public const string Programmatic = "programmatic";
    }
}