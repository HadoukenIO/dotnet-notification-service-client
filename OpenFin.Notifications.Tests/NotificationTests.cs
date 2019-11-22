using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenFin.Notifications.Constants;

namespace OpenFin.Notifications.Tests
{
    [TestClass]
    public class CreateNotificationTests
    {
        Uri uri = new Uri("https://cdn.openfin.co/services/openfin/notifications/app.json");

        [TestMethod]
        public void Initialize_NullInitCompleteHandler_ExceptionThrown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => { NotificationClient.Initialize(uri); });
        }

        [TestMethod]
        public void CreateNotification_CreateNotification_ReturnsOptions()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();            

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Icon = id,
                Category = id,
                Buttons = new ButtonOptions[] { }
            };           

            NotificationClient.OnInitComplete += async () =>
            {
                var notification = await NotificationClient.CreateNotificationAsync(id, options);
                Assert.AreEqual(id, notification.Id);
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateNotification_CreateNotificationWithoutTitle_ThrowsException()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Body = id,
                Icon = id,
                Category = id,
                Buttons = new ButtonOptions[] { }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateNotification_CreateNotificationWithoutBody_ThrowsException()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Icon = id,
                Category = id,
                Buttons = new ButtonOptions[] { }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateNotification_CreateNotificationWithoutIcon_ThrowsException()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Category = id,
                Buttons = new ButtonOptions[] { }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateNotification_CreateNotificationWithoutCategory_ThrowsException()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,                
                Icon = id,
                Buttons = new ButtonOptions[] { }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateNotification_CreateNotificationWithoutButtons_ThrowsException()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Icon = id,
                Category = id,                
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
        }

        [TestMethod]
        public async Task CreateNotification_CreateExpiringNotification_ExpiresSuccessfully()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Icon = id,
                Category = id,
                Expires = DateTime.Now.AddSeconds(10),
                Buttons = new ButtonOptions[] { }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.NotificationClosed += (@event) =>
            {
                Assert.AreEqual(id, @event.NotificationOptions.Id);           
            };            

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();            
        }

        [TestMethod]
        public async Task CreateNotification_CreateExpiringNotificationWithHandler_CallsNotificationActionEventHandler()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Icon = id,
                Category = id,
                Expires = DateTime.Now.AddSeconds(10),
                Buttons = new ButtonOptions[] { },
                OnNotificationExpired = new Dictionary<string, object>
                {
                    { "foo" , "bar" }
                }
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.NotificationActionOccurred += (@event) =>
            {
                Assert.AreEqual(ActionTriggers.Expire, @event.ActionTrigger);
                Assert.IsTrue(@event.NotificationActionResult.ContainsKey("foo"));
                Assert.AreEqual("bar", @event.NotificationActionResult["foo"]);                
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();
        }

        [TestMethod]
        public async Task ClearNotification_ClearingCreatedNotification_TriggersClosedEvent()
        {
            var are = new AutoResetEvent(false);

            string id = Guid.NewGuid().ToString();

            var options = new NotificationOptions
            {
                Title = id,
                Body = id,
                Icon = id,
                Category = id,
                Buttons = new ButtonOptions[] { }                
            };

            NotificationClient.OnInitComplete += () =>
            {
                are.Set();
            };

            NotificationClient.NotificationClosed += (@event) =>
            {
                Assert.AreEqual(id, @event.NotificationOptions.Id);                
                are.Set();
            };

            NotificationClient.NotificationCreated += async (@event) =>
            {
                Assert.AreEqual(id, @event.NotificationOptions.Id);
                await NotificationClient.ClearNotificationAsync(id);
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();
        }

        

    }
}
