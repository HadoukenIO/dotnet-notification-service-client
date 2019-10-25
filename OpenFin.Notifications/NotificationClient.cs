using Newtonsoft.Json.Linq;
using Openfin.Desktop;
using Openfin.Desktop.Messaging;
using OpenFin.Notifications.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFin.Notifications
{
    public class NotificationClient
    {
        private static Runtime _runtime;
        private static ChannelClient _channelClient;

        /// <summary>
        /// 
        /// </summary>
        public static Action<NotificationEvent> NotificationClosed;

        /// <summary>
        /// 
        /// </summary>
        public static Action<NotificationEvent> NotificationCreated;

        /// <summary>
        /// 
        /// </summary>
        public static Action<NotificationEvent> NotificationActionOccurred;

        /// <summary>
        /// 
        /// </summary>
        public static Action InitializationComplete;

        /// <summary>
        /// 
        /// </summary>
        public static void Initialize()
        {
            Initialize(new Uri(ServiceConstants.NotificationsServiceManifestUrl));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestUri"></param>
        public static void Initialize(Uri manifestUri)
        {
            if(InitializationComplete == null)
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
                        InitializationComplete.Invoke();

                    });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Task<object> CreateNotificationAsync(string notificationId, NotificationOptions options)
        {
            options.Id = notificationId;
            return _channelClient?.DispatchAsync<object>(ApiTopics.CreateNotification, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<object> ClearNotificationAsync(string id)
        {
            return _channelClient?.DispatchAsync<object>(ApiTopics.ClearNotification, new { id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<NotificationOptions[]> GetAllAppNotifications()
        {
            return _channelClient?.DispatchAsync<NotificationOptions[]>(ApiTopics.GetAppNotifications, JValue.CreateUndefined());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task<object> ClearAllNotificationsAsync()
        {
            return _channelClient?.DispatchAsync<object>(ApiTopics.ClearAppNotifications, JValue.CreateUndefined());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task ToggleNotificationCenterAsync()
        {
            return _channelClient.DispatchAsync(ApiTopics.ToggleNotificationCenter, JValue.CreateUndefined());
        }
    }
}
