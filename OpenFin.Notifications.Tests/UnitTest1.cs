using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenFin.Notifications.Tests
{
    [TestClass]
    public class NotificationClientTests
    {
        [TestMethod]
        public void Initialize_NullInitCompleteHandler_ExceptionThrown()
        {
            Assert.ThrowsException<InvalidOperationException>(() => { NotificationClient.Initialize(); });
        }

        [TestMethod]
        public void Test2()
        {
            
            NotificationClient.OnInitComplete += () =>
            {
                NotificationClient.CreateNotificationAsync()
            };

            NotificationClient.Initialize();
        }
    }
}
