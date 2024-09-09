using System;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkPlayerCallbacks", menuName = "ScriptableObjects/Network/NetworkPlayerCallbacks")]
public class NetworkPlayerCallbacksSO : ScriptableObject
{
    public event Action<NetworkRunner, PlayerRef> PlayerSpawn;

    public void OnPlayerSpawn(NetworkRunner runner, PlayerRef player)
    {
        PlayerSpawn?.Invoke(runner, player);
    }
}