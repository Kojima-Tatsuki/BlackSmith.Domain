#nullable enable

using System;
using NUnit.Framework;
using BlackSmith.Domain.Networking.Connection;
using BlackSmith.Domain.Networking.Auth;
using BlackSmith.Usecase.Interface.Networking.Connection;

namespace BlackSmith.Domain.Tests.Networking.Connection
{
    [TestFixture]
    public class ConnectionStateMachineTest
    {
        private class MockActionExecutor : IConnectionStateActionExecutor
        {
            public ConnectionStateType? LastEnteredState { get; private set; }
            public ConnectionEvent? LastEnteredEvent { get; private set; }
            public ConnectionStateType? LastExitedState { get; private set; }
            public int EnterCallCount { get; private set; }
            public int ExitCallCount { get; private set; }

            public void ExecuteEnter(ConnectionStateType stateType, ConnectionEvent triggerEvent)
            {
                LastEnteredState = stateType;
                LastEnteredEvent = triggerEvent;
                EnterCallCount++;
            }

            public void ExecuteExit(ConnectionStateType stateType)
            {
                LastExitedState = stateType;
                ExitCallCount++;
            }

            public void Reset()
            {
                LastEnteredState = null;
                LastEnteredEvent = null;
                LastExitedState = null;
                EnterCallCount = 0;
                ExitCallCount++;
            }
        }

        [Test]
        public void Constructor_InitializesWithGivenState()
        {
            // Arrange & Act
            var machine = new ConnectionStateMachine(ConnectionStateType.Offline);

            // Assert
            Assert.AreEqual(ConnectionStateType.Offline, machine.CurrentState.StateType);
            Assert.IsNotNull(machine.CurrentState.Timestamp);
        }

        [Test]
        public void ProcessEvent_ValidTransition_Succeeds()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.Offline);
            var executor = new MockActionExecutor();
            var ev = new ClientConnectedEvent(AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12"));

            // Act
            var result = machine.ProcessEvent(ev, executor);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ConnectionStateType.Offline, result.PreviousState?.StateType);
            Assert.AreEqual(ConnectionStateType.ClientConnecting, result.NewState?.StateType);
            Assert.AreEqual(ev, result.TriggerEvent);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void ProcessEvent_InvalidTransition_ReturnsNoTransition()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.Offline);
            var executor = new MockActionExecutor();
            var ev = new TransportFailureEvent(); // Offline状態からTransportFailureEventは遷移なし

            // Act
            var result = machine.ProcessEvent(ev, executor);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(ConnectionStateType.Offline, result.PreviousState?.StateType);
            Assert.IsNull(result.NewState);
            Assert.AreEqual(ev, result.TriggerEvent);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [Test]
        public void ProcessEvent_CallsExitThenEnter()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.ClientConnecting);
            var executor = new MockActionExecutor();
            var ev = new ClientConnectedEvent(AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12"));

            // Act
            machine.ProcessEvent(ev, executor);

            // Assert
            Assert.AreEqual(1, executor.ExitCallCount);
            Assert.AreEqual(1, executor.EnterCallCount);
            Assert.AreEqual(ConnectionStateType.ClientConnecting, executor.LastExitedState);
            Assert.AreEqual(ConnectionStateType.ClientConnected, executor.LastEnteredState);
        }

        [Test]
        public void ProcessEvent_ClientDisconnected_WithReason_TransitionsToOffline()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.ClientConnected);
            var executor = new MockActionExecutor();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var ev = new ClientDisconnectedEvent(playerId, "UserRequestedDisconnect");

            // Act
            var result = machine.ProcessEvent(ev, executor);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ConnectionStateType.Offline, result.NewState?.StateType);
        }

        [Test]
        public void ProcessEvent_ClientDisconnected_WithoutReason_TransitionsToReconnecting()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.ClientConnected);
            var executor = new MockActionExecutor();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var ev = new ClientDisconnectedEvent(playerId, null);

            // Act
            var result = machine.ProcessEvent(ev, executor);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ConnectionStateType.ClientReconnecting, result.NewState?.StateType);
        }

        [Test]
        public void ProcessEvent_ClientDisconnected_WithEmptyReason_TransitionsToReconnecting()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.ClientConnected);
            var executor = new MockActionExecutor();
            var playerId = AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12");
            var ev = new ClientDisconnectedEvent(playerId, "");

            // Act
            var result = machine.ProcessEvent(ev, executor);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ConnectionStateType.ClientReconnecting, result.NewState?.StateType);
        }

        [Test]
        public void ProcessEvent_UpdatesCurrentState()
        {
            // Arrange
            var machine = new ConnectionStateMachine(ConnectionStateType.StartingHost);
            var executor = new MockActionExecutor();
            var ev = new ServerStartedEvent();

            // Act
            machine.ProcessEvent(ev, executor);

            // Assert
            Assert.AreEqual(ConnectionStateType.Hosting, machine.CurrentState.StateType);
        }

        [Test]
        public void StateTransitionResult_Success_HasCorrectProperties()
        {
            // Arrange
            var previousState = new ConnectionState(ConnectionStateType.Offline, DateTime.UtcNow);
            var newState = new ConnectionState(ConnectionStateType.ClientConnecting, DateTime.UtcNow);
            var ev = new ClientConnectedEvent(AuthPlayerId.CreateId("abcdefghijklmnopqrstuvwxyz12"));

            // Act
            var result = StateTransitionResult.Success(previousState, newState, ev);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(previousState, result.PreviousState);
            Assert.AreEqual(newState, result.NewState);
            Assert.AreEqual(ev, result.TriggerEvent);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void StateTransitionResult_NoTransition_HasCorrectProperties()
        {
            // Arrange
            var currentState = new ConnectionState(ConnectionStateType.Offline, DateTime.UtcNow);
            var ev = new TransportFailureEvent();

            // Act
            var result = StateTransitionResult.NoTransition(currentState, ev);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(currentState, result.PreviousState);
            Assert.IsNull(result.NewState);
            Assert.AreEqual(ev, result.TriggerEvent);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.That(result.ErrorMessage, Does.Contain("No transition defined"));
        }
    }
}
