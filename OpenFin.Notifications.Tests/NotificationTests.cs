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

        private void cleanup()
        {
            NotificationClient.NotificationActionOccurred = null;
            NotificationClient.OnInitComplete = null;
            NotificationClient.NotificationClosed = null;
            NotificationClient.NotificationCreated = null;
        }
        [TestInitialize]
        public void Init()
        {
            cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            cleanup();
        }

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

            var createdNotificationId = "";

            NotificationClient.OnInitComplete = async () =>
            {
                var notification = await NotificationClient.CreateNotificationAsync(id, options);
                createdNotificationId = notification.Id;
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            Assert.AreEqual(id, createdNotificationId);
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

        [Ignore]        
        public async Task CreateNotification_CreateExpiringNotificationWithoutHandler_ExpiresSuccessfully()
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

            NotificationClient.OnInitComplete = () =>
            {
                are.Set();
            };

            var closedNotificationId = string.Empty;

            NotificationClient.NotificationClosed = (@event) =>
            {
                closedNotificationId = @event.NotificationOptions.Id;
                are.Set();
            };           

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();
            Assert.AreEqual(id, closedNotificationId);
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

            NotificationClient.OnInitComplete = () =>
            {
                are.Set();
            };

            var eventActionTrigger = "";
            var containsFooKey = false;
            var fooValue = "";

            NotificationClient.NotificationActionOccurred += (@event) =>
            {
                eventActionTrigger = @event.ActionTrigger;
                containsFooKey = @event.NotificationActionResult.ContainsKey("foo");
                fooValue = @event.NotificationActionResult["foo"].ToString();
                            
                are.Set();
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();

            Assert.AreEqual(ActionTriggers.Expire, eventActionTrigger);
            Assert.IsTrue(containsFooKey);
            Assert.AreEqual("bar", fooValue);
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

            NotificationClient.OnInitComplete = () =>
            {
                are.Set();
            };

            var closedNotificationId = "";

            NotificationClient.NotificationClosed = (@event) =>
            {
                closedNotificationId = @event.NotificationOptions.Id;
                are.Set();
            };

            var createdNotificationId = "";
            NotificationClient.NotificationCreated = async (@event) =>
            {
                createdNotificationId = @event.NotificationOptions.Id;
                await NotificationClient.ClearNotificationAsync(id);
            };

            NotificationClient.Initialize(uri);
            are.WaitOne();
            await NotificationClient.CreateNotificationAsync(id, options);
            are.WaitOne();

            Assert.AreEqual(id, createdNotificationId);
            Assert.AreEqual(id, closedNotificationId);
        }
    }
}
