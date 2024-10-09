using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class NetworkManager : SingletonBase<NetworkManager>, INetworkRunnerCallbacks
    {
        [Header("SOs")] [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
        [Header("Other")] [SerializeField] private NetworkRunner runnerPrefab;
        [SerializeField] private NetworkSceneManagerDefault networkSceneManagerDefault;

        public bool RunnerInitialized { get; private set; }
        public NetworkRunner NetworkRunner { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        async void Start()
        {
            CreateRunner();
            RunnerInitialized = true;
            await Connect();
        }

        private void CreateRunner()
        {
            NetworkRunner = Instantiate(runnerPrefab, transform).GetComponent<NetworkRunner>();
            
            NetworkRunner.AddCallbacks(this);
        }

        private async Task Connect()
        {
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = "TestSession",
                SceneManager = networkSceneManagerDefault,
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            };

            var result = await NetworkRunner.StartGame(args);

            if (result.Ok)
                Debug.Log("StartGame successful");
            else
                Debug.LogError($"Error: {result.ErrorMessage}");

            //eventsChannel.OnConnectedToRoom();
        }

        #region NetworkRunnerCallbacks

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            networkRunnerCallbacks.OnObjectExitAOI(runner, obj, player);
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            networkRunnerCallbacks.OnObjectEnterAOI(runner, obj, player);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            networkRunnerCallbacks.OnPlayerJoined(runner, player);
            Debug.Log("A new player joined to the session");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            networkRunnerCallbacks.OnPlayerLeft(runner, player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            networkRunnerCallbacks.OnInput(runner, input);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            networkRunnerCallbacks.OnInputMissing(runner, player, input);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            networkRunnerCallbacks.OnShutdown(runner, shutdownReason);
            Debug.Log("Runner Shutdown");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            networkRunnerCallbacks.OnConnectedToServer(runner);
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            networkRunnerCallbacks.OnDisconnectedFromServer(runner, reason);
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
            networkRunnerCallbacks.OnConnectRequest(runner, request, token);
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            networkRunnerCallbacks.OnConnectFailed(runner, remoteAddress, reason);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            networkRunnerCallbacks.OnUserSimulationMessage(runner, message);
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            networkRunnerCallbacks.OnSessionListUpdated(runner, sessionList);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            networkRunnerCallbacks.OnCustomAuthenticationResponse(runner, data);
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            networkRunnerCallbacks.OnHostMigration(runner, hostMigrationToken);
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
            networkRunnerCallbacks.OnReliableDataReceived(runner, player, key, data);
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            networkRunnerCallbacks.OnReliableDataProgress(runner, player, key, progress);
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            networkRunnerCallbacks.OnSceneLoadDone(runner);
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            networkRunnerCallbacks.OnSceneLoadStart(runner);
        }

        #endregion
    }
}



