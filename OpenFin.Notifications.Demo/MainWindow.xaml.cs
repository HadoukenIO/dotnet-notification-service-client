using Newtonsoft.Json;
using OpenFin.Notifications.Constants;
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
        private const string NOTIFICATION_TITLE = "WPF Notifications Demo App";

        public MainWindow()
        {
            InitializeComponent();

            NotificationClient.NotificationClosed += NotificationClient_NotificationClosed;
            NotificationClient.NotificationCreated += NotificationClient_NotificationCreated;
            NotificationClient.NotificationActionOccurred += NotificationClient_NotificationActionOccurred;

            NotificationClient.OnInitComplete += () =>
            {
                toggleButtons(true);

                Dispatcher.Invoke(() =>
                {
                    bodyContentTypeSelector.SelectedIndex = 0;
                });
            };

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
            log($"Notification action occurred fired by trigger ({@event.ActionTrigger}). {JsonConvert.SerializeObject(@event.NotificationActionResult)}\n");

            if (@event.ActionTrigger == ActionTriggers.Control)
            {
                if (@event.NotificationActionResult.ContainsKey("btn"))
                {
                    log($"{@event.NotificationActionResult["btn"]} from NotificationId {@event.NotificationOptions.Id} clicked!");
                }
            }
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
                messageBox.ScrollToEnd();
            });
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (sender as FrameworkElement).Name.Substring("create".Length);

            try
            {
                await NotificationClient.CreateNotificationAsync($"wpf/{id}", new NotificationOptions
                {
                    Title = $"{NOTIFICATION_TITLE}-wpf/{id}",
                    Body = NotificationBodyService.GetNotificationBodyContent((BodyContentType)bodyContentTypeSelector.SelectedIndex),
                    Category = "Category",
                    Icon = "https://openfin.co/favicon-32x32.png",
                    OnNotificationSelect = new Dictionary<string, object>
                    {
                        {"task", "selected" }
                    },
                    Buttons = NotificationBodyService.GenerateButtons((int)btnCount.Value)
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
        }

        private async void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            var fetchResult = await NotificationClient.GetAllAppNotificationsAsync();
            log($"Fetched {fetchResult.Count()} notifications");
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

        private void btnCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblButtonCount.Content = $"Button Count ({e.NewValue})";
        }

        private void messageBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            messageBox.Text = string.Empty;
        }
    }
}