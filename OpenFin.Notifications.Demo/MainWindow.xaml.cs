using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenFin.Notifications.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string EXPIRING_NOTIFICATION_ID = "wpf/expiringnotification";
        private const string NOTIFICATION_TITLE       = "WPF Notifications Demo App";
        public MainWindow()
        {
            InitializeComponent();
            
            NotificationClient.NotificationClosed         += NotificationClient_NotificationClosed;
            NotificationClient.NotificationCreated        += NotificationClient_NotificationCreated;
            NotificationClient.NotificationActionOccurred += NotificationClient_NotificationActionOccurred;

            NotificationClient.InitializationComplete += () => { toggleButtons(true); };
            NotificationClient.Initialize();
            toggleButtons(false);
        }

        private void toggleButtons(bool isEnabled)
        {
            Dispatcher.Invoke(() =>
            {
                fetchButton.IsEnabled = clearAllButton.IsEnabled = toggleButton.IsEnabled = isEnabled;

                foreach (FrameworkElement item in ((Panel)create1.Parent).Children)
                {
                    if (item is Button)
                    {
                        item.IsEnabled = isEnabled;
                    }
                }
            });
        }

        private void NotificationClient_NotificationActionOccurred(NotificationEvent @event)
        {
            
            log($"Notification action occurred fired. {JsonConvert.SerializeObject(@event.NotificationActionResult)}\n");
        }

        private void NotificationClient_NotificationCreated(NotificationEvent @event)
        {
            log($"Notification {@event.NotificationOptions.Id} created.\n");
        }

        private void NotificationClient_NotificationClosed(NotificationEvent @event)
        {
            log($"Notification {@event.NotificationOptions.Id} closed.\n");
        }      

        private void log(string text)
        {
            Dispatcher.Invoke(() =>
            {
                messageBox.Text += text;
            });
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (sender as FrameworkElement).Name.Substring("create".Length);

            try
            {
                await NotificationClient.CreateNotificationAsync($"wpf/{id}", new NotificationOptions
                {
                    Title = NOTIFICATION_TITLE,
                    Body = $"Notification {id} Body ",
                    Category = "Category",
                    Icon = "https://openfin.co/favicon-32x32.png",
                    OnNotificationSelect = new Dictionary<string, object>
                    {
                        {"task", "selected" }
                    },
                    Buttons = new[]
            {
                    new ButtonOptions() {
                        Title = "Button1",
                        IconUrl = "https://openfin.co/favicon-32x32.png",
                        OnNotificationButtonClick = new Dictionary<string, object>
                        {
                            { "btn", "button1" }
                        }},
                    new ButtonOptions() { Title = "Button2",
                    OnNotificationButtonClick = new Dictionary<string, object>
                    {
                        { "btn", "button2" }
                    }
                    }
                }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (sender as FrameworkElement).Name.Substring("close".Length);
            NotificationClient.ClearNotificationAsync($"wpf/{id}");
            log("Notifications cleared.");
        }

        private async void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            var fetchResult = await NotificationClient.GetAllAppNotifications();
            log($"Fetched {fetchResult.Count()} notifications");
        }

        private void messageBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            messageBox.Text = string.Empty;
        }

        private void ToggleNotifications_Click(object sender, RoutedEventArgs e)
        {
            NotificationClient.ToggleNotificationCenterAsync();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            NotificationClient.ClearAllNotificationsAsync();
        }

        private void CloseExpiring_Click(object sender, RoutedEventArgs e)
        {
            NotificationClient.ClearNotificationAsync(EXPIRING_NOTIFICATION_ID);
        }



        private void CreateExpiring_Click(object sender, RoutedEventArgs e)
        {
            var options = new NotificationOptions
            {
                Body = "# This notification will expire in _10 seconds_.",
                Title = NOTIFICATION_TITLE,
                Category = "Expiring Notification",
                Buttons = new[]
            {
                    new ButtonOptions() { Title = "Button1", IconUrl = "https://openfin.co/favicon-32x32.png" },
                    new ButtonOptions() { Title = "Button2"}
                },
                Icon = "https://openfin.co/favicon-32x32.png",
                Expires = DateTime.Now.AddSeconds(30),
                OnNotificationExpired = new Dictionary<string, object>
                {
                    { "foo" , "bar" }
                }
            };

            NotificationClient.CreateNotificationAsync(EXPIRING_NOTIFICATION_ID, options);
        }
    }
}