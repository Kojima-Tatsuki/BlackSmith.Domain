#nullable enable

using System;
using NUnit.Framework;
using BlackSmith.Domain.Networking.Connection;

namespace BlackSmith.Domain.Tests.Networking.Connection
{
    [TestFixture]
    public class ConnectionStateTest
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var stateType = ConnectionStateType.ClientConnected;
            var timestamp = DateTime.UtcNow;

            // Act
            var connectionState = new ConnectionState(stateType, timestamp);

            // Assert
            Assert.AreEqual(stateType, connectionState.StateType);
            Assert.AreEqual(timestamp, connectionState.Timestamp);
        }

        [Test]
        public void Record_IsImmutable()
        {
            // Arrange
            var state1 = new ConnectionState(ConnectionStateType.Offline, DateTime.UtcNow);
            var state2 = state1 with { StateType = ConnectionStateType.ClientConnected };

            // Assert
            Assert.AreNotEqual(state1.StateType, state2.StateType);
            Assert.AreEqual(ConnectionStateType.Offline, state1.StateType);
            Assert.AreEqual(ConnectionStateType.ClientConnected, state2.StateType);
        }

        [Test]
        public void Equality_WorksCorrectly()
        {
            // Arrange
            var timestamp = DateTime.UtcNow;
            var state1 = new ConnectionState(ConnectionStateType.Hosting, timestamp);
            var state2 = new ConnectionState(ConnectionStateType.Hosting, timestamp);
            var state3 = new ConnectionState(ConnectionStateType.ClientConnected, timestamp);

            // Assert
            Assert.AreEqual(state1, state2);
            Assert.AreNotEqual(state1, state3);
        }

        [Test]
        public void AllStateTypes_CanBeConstructed()
        {
            // Arrange & Act & Assert
            var timestamp = DateTime.UtcNow;

            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.Offline, timestamp));
            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.ClientConnecting, timestamp));
            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.ClientConnected, timestamp));
            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.ClientReconnecting, timestamp));
            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.StartingHost, timestamp));
            Assert.DoesNotThrow(() => new ConnectionState(ConnectionStateType.Hosting, timestamp));
        }
    }
}
