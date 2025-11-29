#nullable enable

using System;
using NUnit.Framework;
using BlackSmith.Domain.Networking.Connection;
using BlackSmith.Domain.Networking.Auth;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Tests.Networking.Connection
{
    [TestFixture]
    public class ConnectionEventTest
    {
        [Test]
        public void ClientConnectedEvent_HasCorrectProperties()
        {
            // Arrange
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var beforeCreate = DateTime.UtcNow;

            // Act
            var ev = new ClientConnectedEvent(playerId);

            // Assert
            Assert.AreEqual(playerId, ev.PlayerId);
            Assert.GreaterOrEqual(ev.OccurredAt, beforeCreate);
            Assert.LessOrEqual(ev.OccurredAt, DateTime.UtcNow);
        }

        [Test]
        public void ClientDisconnectedEvent_WithReason_HasCorrectProperties()
        {
            // Arrange
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var reason = "User requested disconnect";

            // Act
            var ev = new ClientDisconnectedEvent(playerId, reason);

            // Assert
            Assert.AreEqual(playerId, ev.PlayerId);
            Assert.AreEqual(reason, ev.Reason);
            Assert.IsNotNull(ev.OccurredAt);
        }

        [Test]
        public void ClientDisconnectedEvent_WithoutReason_HasNullReason()
        {
            // Arrange
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");

            // Act
            var ev = new ClientDisconnectedEvent(playerId, null);

            // Assert
            Assert.AreEqual(playerId, ev.PlayerId);
            Assert.IsNull(ev.Reason);
        }

        [Test]
        public void ServerStartedEvent_CanBeCreated()
        {
            // Act
            var ev = new ServerStartedEvent();

            // Assert
            Assert.IsNotNull(ev);
            Assert.IsNotNull(ev.OccurredAt);
        }

        [Test]
        public void ServerStoppedEvent_HasIsHostProperty()
        {
            // Act
            var hostEvent = new ServerStoppedEvent(IsHost: true);
            var clientEvent = new ServerStoppedEvent(IsHost: false);

            // Assert
            Assert.IsTrue(hostEvent.IsHost);
            Assert.IsFalse(clientEvent.IsHost);
        }

        [Test]
        public void TransportFailureEvent_CanBeCreated()
        {
            // Act
            var ev = new TransportFailureEvent();

            // Assert
            Assert.IsNotNull(ev);
            Assert.IsNotNull(ev.OccurredAt);
        }

        [Test]
        public void UserRequestedShutdownEvent_CanBeCreated()
        {
            // Act
            var ev = new UserRequestedShutdownEvent();

            // Assert
            Assert.IsNotNull(ev);
            Assert.IsNotNull(ev.OccurredAt);
        }

        [Test]
        public void ConnectionApprovalRequestEvent_HasCorrectProperties()
        {
            // Arrange
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var characterName = new CharacterName("TestCharacter");

            // Act
            var ev = new ConnectionApprovalRequestEvent(playerId, characterName);

            // Assert
            Assert.AreEqual(playerId, ev.PlayerId);
            Assert.AreEqual(characterName, ev.CharacterName);
            Assert.IsNotNull(ev.OccurredAt);
        }

        [Test]
        public void OccurredAt_IsSetAutomatically()
        {
            // Arrange
            var beforeCreate = DateTime.UtcNow;

            // Act
            var ev = new ServerStartedEvent();
            var afterCreate = DateTime.UtcNow;

            // Assert
            Assert.GreaterOrEqual(ev.OccurredAt, beforeCreate);
            Assert.LessOrEqual(ev.OccurredAt, afterCreate);
        }

        [Test]
        public void Events_AreRecords()
        {
            // Arrange
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var ev1 = new ClientConnectedEvent(playerId);
            var ev2 = new ClientConnectedEvent(playerId);

            // Act & Assert - record型は構造的等価性を持つ
            // ただし、OccurredAtが異なるため等価にはならない
            Assert.AreNotEqual(ev1, ev2);
            Assert.AreEqual(ev1.PlayerId, ev2.PlayerId);
        }
    }
}
