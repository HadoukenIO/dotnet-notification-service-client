using Newtonsoft.Json.Linq;
using Openfin.Desktop;
using Openfin.Desktop.Messaging;
using OpenFin.Notifications.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenFin.Notifications
{
    /// <summary>
    /// The client interface that initializes and manages notifications.
    /// </summary>
    public static class NotificationClient
    {
        private static Runtime _runtime;
        private static ChannelClient _channelClient;

        /// <summary>
        /// The action delegate to be called when a notification is closed.
        /// </summary>
        public static Action<NotificationEvent> NotificationClosed;

        /// <summary>
        /// The action delegate to be called when a notification is created.
        /// </summary>
        public static Action<NotificationEvent> NotificationCreated;

        /// <summary>
        /// The action delegate to be called when a notification action occurs.
        /// </summary>
        public static Action<NotificationEvent> NotificationActionOccurred;

        /// <summary>
        /// The action delegate to be called when the service initialization completes. (Required)
        /// </summary>
        public static Action OnInitComplete;

        /// <summary>
        /// Initializes the Notification Service.
        /// <event cref="OnInitComplete">The action delegate called when service initialization completes successfully.</event>
        /// <exception cref="InvalidOperationException">This exception gets thrown if the OnInitComplete
        /// action delegate does not get set before calling this method.
        /// </exception>
        /// </summary>
        public static void Initialize()
        {
            Initialize(new Uri(ServiceConstants.NotificationsServiceManifestUrl));
        }

        /// <summary>
        /// Initializes the Notification Service.
        /// </summary>
        /// <param name="manifestUri">The Uri pointing to the notification service manifest.</param>
        public static void Initialize(Uri manifestUri)
        {
            if (OnInitComplete == null)
            {
                throw new InvalidOperationException("InitializationComplete handler must be registered before calling Initialize()");
            }

            var runtimeOptions = RuntimeOptions.LoadManifest(manifestUri);
            runtimeOptions.Arguments += " --inspect";

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var productAttributes = entryAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true);

            if (productAttributes.Length > 0)
            {
                runtimeOptions.UUID = ((System.Reflection.AssemblyProductAttribute)productAttributes[0]).Product;
            }
            else
            {
                runtimeOptions.UUID = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            }

            _runtime = Runtime.GetRuntimeInstance(runtimeOptions);

            _runtime.Connect(() =>
            {
                var notificationsService = _runtime.CreateApplication(runtimeOptions.StartupApplicationOptions);

                notificationsService.isRunning(
                    async ack =>
                    {
                        if (!(bool)(ack.getData() as JValue).Value)
                        {
                            notificationsService.run();
                        }

                        _channelClient = _runtime.InterApplicationBus.Channel.CreateClient(ServiceConstants.NotificationServiceChannelName);

                        _channelClient.RegisterTopic<NotificationEvent>(ChannelTopics.Events, (@event) =>
                        {
                            switch (@event.EventType)
                            {
                                case NotificationEventTypes.NotificationAction:
                                    NotificationActionOccurred?.Invoke(@event);
                                    break;

                                case NotificationEventTypes.NotificationClosed:
                                    NotificationClosed?.Invoke(@event);
                                    break;

                                case NotificationEventTypes.NotificationCreated:
                                    NotificationCreated?.Invoke(@event);
                                    break;

                                default:
                                    throw new ArgumentException($"Invalid event type : {@event.EventType}");
                            }
                        });

                        await _channelClient.ConnectAsync();
                        await _channelClient.DispatchAsync(ApiTopics.AddEventListener, NotificationEventTypes.NotificationAction);
                        OnInitComplete.Invoke();
                    });
            });
        }

        /// <summary>
        /// Creates a notification.
        /// </summary>
        /// <param name="notificationId">The notification Id.</param>
        /// <param name="options">The notification options.</param>
        /// <returns></returns>
        public static Task<NotificationOptions> CreateNotificationAsync(string notificationId, NotificationOptions options)
        {
            options.Id = notificationId;
            return _channelClient?.DispatchAsync<NotificationOptions>(ApiTopics.CreateNotification, options);
        }        

        /// <summary>
        /// Removes a specific notification from the notification center.
        /// </summary>
        /// <param name="id">The Id of the notification to be removed.</param>
        /// <returns>A value denoting if the action was successful or not.</returns>
        public static Task<bool> ClearNotificationAsync(string id)
        {
            return _channelClient?.DispatchAsync<bool>(ApiTopics.ClearNotification, new { id });
        }

        /// <summary>
        /// Retrieves all notifications from the notification center.
        /// </summary>
        /// <returns>The notifications.</returns>
        public static Task<IEnumerable<NotificationOptions>> GetAllAppNotificationsAsync()
        {
            return _channelClient?.DispatchAsync<IEnumerable<NotificationOptions>>(ApiTopics.GetAppNotifications, JValue.CreateUndefined());
        }

        /// <summary>
        /// Removes all notifications from the notification center.
        /// </summary>
        /// <returns>A task</returns>
        public static Task ClearAllNotificationsAsync()
        {
            return _channelClient?.DispatchAsync(ApiTopics.ClearAppNotifications, JValue.CreateUndefined());
        }

        /// <summary>
        /// Toggles the visibility of the notification center.
        /// </summary>
        /// <returns>A task</returns>
        public static Task ToggleNotificationCenterAsync()
        {
            return _channelClient.DispatchAsync(ApiTopics.ToggleNotificationCenter, JValue.CreateUndefined());
        }
    }
}