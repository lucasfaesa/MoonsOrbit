using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Networking
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("SOs")] 
        [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
        [SerializeField] private NetworkPlayerCallbacksSO networkPlayerCallbacks;
        [Header("Other")] 
        [SerializeField] private NetworkPrefabRef playerPrefab;

        private void OnEnable()
        {
            networkRunnerCallbacks.PlayerJoined += OnPlayerJoined;
        }

        private void OnDisable()
        {
            networkRunnerCallbacks.PlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(NetworkRunner networkRunner, PlayerRef playerRef)
        {
            if (playerRef == networkRunner.LocalPlayer)
            {
                Vector3 spawnPos = new Vector3(Random.Range(1, 5), 0, Random.Range(1, 5));

                networkRunner.Spawn(playerPrefab, spawnPos, Quaternion.identity, playerRef);
            
                networkPlayerCallbacks.OnPlayerSpawn(networkRunner, playerRef);
            }
        }
    }
}

