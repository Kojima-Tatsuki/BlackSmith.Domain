#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using BlackSmith.Domain.Networking.Connection;
using BlackSmith.Domain.Networking.Auth;

namespace BlackSmith.Domain.Tests.Networking.Connection
{
    [TestFixture]
    public class ConnectionApprovalPolicyTest
    {
        [Test]
        public void Approve_Success_WhenConditionsAreMet()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var connectedPlayers = new List<AuthPlayerId>();

            // Act
            var result = policy.Approve(playerId, 0, 8, connectedPlayers);

            // Assert
            Assert.IsTrue(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        public void Approve_RejectsServerFull_WhenMaxConnectionsReached()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var connectedPlayers = new List<AuthPlayerId>();

            // Act
            var result = policy.Approve(playerId, 8, 8, connectedPlayers);

            // Assert
            Assert.IsFalse(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.ServerFull, result.Status);
        }

        [Test]
        public void Approve_RejectsServerFull_WhenExceedsMaxConnections()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var connectedPlayers = new List<AuthPlayerId>();

            // Act
            var result = policy.Approve(playerId, 10, 8, connectedPlayers);

            // Assert
            Assert.IsFalse(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.ServerFull, result.Status);
        }

        [Test]
        public void Approve_RejectsDuplicateLogin_WhenPlayerAlreadyConnected()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var connectedPlayers = new List<AuthPlayerId> { playerId };

            // Act
            var result = policy.Approve(playerId, 1, 8, connectedPlayers);

            // Assert
            Assert.IsFalse(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.LoggedInAgain, result.Status);
        }

        [Test]
        public void Approve_Success_WhenOneSlotRemaining()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var otherPlayer = AuthPlayerId.CreateId("zyxwvutsrqponmlkjihgfedcba21");
            var connectedPlayers = new List<AuthPlayerId> { otherPlayer };

            // Act
            var result = policy.Approve(playerId, 7, 8, connectedPlayers);

            // Assert
            Assert.IsTrue(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        public void Approve_Success_WithEmptyConnectedPlayersList()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var connectedPlayers = new List<AuthPlayerId>();

            // Act
            var result = policy.Approve(playerId, 0, 8, connectedPlayers);

            // Assert
            Assert.IsTrue(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        public void Approve_Success_WhenDifferentPlayerConnected()
        {
            // Arrange
            var policy = new ConnectionApprovalPolicy();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var otherPlayer1 = AuthPlayerId.CreateId("aaaaaaaaaa11111111111111111a");
            var otherPlayer2 = AuthPlayerId.CreateId("bbbbbbbbbb22222222222222222b");
            var connectedPlayers = new List<AuthPlayerId> { otherPlayer1, otherPlayer2 };

            // Act
            var result = policy.Approve(playerId, 2, 8, connectedPlayers);

            // Assert
            Assert.IsTrue(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        public void ConnectionApprovalResult_Approve_CreatesApprovedResult()
        {
            // Act
            var result = ConnectionApprovalResult.Approve();

            // Assert
            Assert.IsTrue(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.Success, result.Status);
        }

        [Test]
        public void ConnectionApprovalResult_Reject_CreatesRejectedResult()
        {
            // Act
            var result = ConnectionApprovalResult.Reject(ConnectionStatus.ServerFull);

            // Assert
            Assert.IsFalse(result.IsApproved);
            Assert.AreEqual(ConnectionStatus.ServerFull, result.Status);
        }
    }
}
