using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkRunnerCallbacks", menuName = "ScriptableObjects/Network/NetworkRunnerCallbacks")]
public class NetworkRunnerCallbacksSO : ScriptableObject
{
    public event Action<NetworkRunner, NetworkObject, PlayerRef> ObjectExitAOI; 
    public event Action<NetworkRunner, NetworkObject, PlayerRef> ObjectEnterAOI; 
    public event Action<NetworkRunner, PlayerRef> PlayerJoined; 
    public event Action<NetworkRunner, PlayerRef> PlayerLeft;
    public event Action<NetworkRunner, NetworkInput> Input;
    public event Action<NetworkRunner, PlayerRef, NetworkInput> InputMissing;
    public event Action<NetworkRunner, ShutdownReason> Shutdown;
    public event Action<NetworkRunner> ConnectedToServer;
    public event Action<NetworkRunner, NetDisconnectReason> DisconnectedFromServer;
    public event Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> ConnectRequest;
    public event Action<NetworkRunner, NetAddress, NetConnectFailedReason> ConnectFailed;
    public event Action<NetworkRunner, SimulationMessagePtr> UserSimulationMessage;
    public event Action<NetworkRunner, List<SessionInfo>> SessionListUpdated;
    public event Action<NetworkRunner, Dictionary<string, object>> CustomAuthenticationResponse;
    public event Action<NetworkRunner, HostMigrationToken> HostMigration;
    public event Action<NetworkRunner, PlayerRef, ReliableKey, ArraySegment<byte>> ReliableDataReceived;
    public event Action<NetworkRunner, PlayerRef, ReliableKey, float> ReliableDataProgress;
    public event Action<NetworkRunner> SceneLoadDone;
    public event Action<NetworkRunner> SceneLoadStart;
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        ObjectExitAOI?.Invoke(runner, networkObject, player);
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player)
    {
        ObjectEnterAOI?.Invoke(runner, networkObject, player);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        PlayerJoined?.Invoke(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        PlayerLeft?.Invoke(runner, player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        Input?.Invoke(runner, networkInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput networkInput)
    {
        InputMissing?.Invoke(runner, player, networkInput);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Shutdown?.Invoke(runner, reason);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        ConnectedToServer?.Invoke(runner);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        DisconnectedFromServer?.Invoke(runner, reason);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest connectRequest, byte[] value)
    {
        ConnectRequest?.Invoke(runner, connectRequest, value);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress address, NetConnectFailedReason reason)
    {
        ConnectFailed?.Invoke(runner, address, reason);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        UserSimulationMessage?.Invoke(runner, message);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionInfoList)
    {
        SessionListUpdated?.Invoke(runner, sessionInfoList);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> responseData)
    {
        CustomAuthenticationResponse?.Invoke(runner, responseData);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken migrationToken)
    {
        HostMigration?.Invoke(runner, migrationToken);
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        ReliableDataReceived?.Invoke(runner, player, key, data);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        ReliableDataProgress?.Invoke(runner, player, key, progress);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        SceneLoadDone?.Invoke(runner);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        SceneLoadStart?.Invoke(runner);
    }
     
}
