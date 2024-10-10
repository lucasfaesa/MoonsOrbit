using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkPlayerCallbacks", menuName = "ScriptableObjects/Network/NetworkPlayerCallbacks")]
public class NetworkPlayerCallbacksSO : ScriptableObject
{
    public List<PlayerRef> PlayersInGame { get; set; } = new List<PlayerRef>();
    
    public event Action<NetworkRunner, PlayerRef> PlayerSpawn;

    public void OnPlayerSpawn(NetworkRunner runner, PlayerRef player)
    {
        PlayerSpawn?.Invoke(runner, player);
        PlayersInGame.Add(player);
    }

    public void OnPlayerLeft(PlayerRef playerRef)
    {
        PlayersInGame.Remove(playerRef);
    }

    public void Reset()
    {
        PlayersInGame.Clear();
    }
}