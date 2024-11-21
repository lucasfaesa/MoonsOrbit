using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [SerializeField] private GameObject localPlayerPrefab;
        [SerializeField] private NetworkPrefabRef puppetPlayerPrefab;
        [Space] 
        [SerializeField] private List<Transform> spawnPointsTransform = new();

        private List<Transform> _puppetPlayersInGame = new();
        
        private bool _spawned;
        
        private void OnEnable()
        {
            networkRunnerCallbacks.PlayerJoined += OnPlayerJoined;
            networkRunnerCallbacks.PlayerLeft += OnPlayerLeft;
        }

        private void OnDisable()
        {
            networkRunnerCallbacks.PlayerJoined -= OnPlayerJoined;
            networkRunnerCallbacks.PlayerLeft -= OnPlayerLeft;
        }

        private void OnPlayerJoined(NetworkRunner networkRunner, PlayerRef playerRef)
        {
            if (playerRef == networkRunner.LocalPlayer)
            {
                if (networkPlayerCallbacks.PlayersInGame.Count == 0)
                {
                    int randSpawnPointIndex = Random.Range(0, spawnPointsTransform.Count);
                    
                    var localPlayer = Instantiate(localPlayerPrefab, spawnPointsTransform[randSpawnPointIndex]);
                    
                    localPlayer.transform.SetParent(null);
                    
                    networkRunner.Spawn(puppetPlayerPrefab, spawnPointsTransform[randSpawnPointIndex].position, Quaternion.identity, playerRef);
                   
                }
                else
                {
                    var furthestSpawnPoints = GetFurthestSpawnPointsFromOtherPlayers();
                    
                    int randSpawnPointIndex = Random.Range(0, furthestSpawnPoints.Count);
                    
                    var localPlayer = Instantiate(localPlayerPrefab, spawnPointsTransform[randSpawnPointIndex]);
                    
                    localPlayer.transform.SetParent(null);
                    
                    networkRunner.Spawn(puppetPlayerPrefab, spawnPointsTransform[randSpawnPointIndex].position, Quaternion.identity, playerRef);

                }
                
            }
            
            
            RefreshPuppetPlayerList();
            networkPlayerCallbacks.OnPlayerSpawn(networkRunner, playerRef);
        }
        
        private async void RefreshPuppetPlayerList()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            
            _puppetPlayersInGame.Clear();
            
            var puppetPlayersInGame = GameObject.FindGameObjectsWithTag("PuppetPlayer");
            
            Debug.LogError($"Quantity {puppetPlayersInGame.Length} Playerszz");
            
            foreach (var puppetPlayer in puppetPlayersInGame)
            {
                _puppetPlayersInGame.Add(puppetPlayer.transform);
            }
            
            Debug.LogError($"Quantity {_puppetPlayersInGame.Count} Players");
        }
        
        public List<Transform> GetFurthestSpawnPointsFromOtherPlayers()
        {
            // Dictionary to store spawn points and their total distances to all players
            Dictionary<Transform, float> spawnPointDistances = new Dictionary<Transform, float>();

            foreach (var spawnPoint in spawnPointsTransform)
            {
                float totalDistance = 0f;

                foreach (var player in _puppetPlayersInGame)
                {
                    totalDistance += Vector3.Distance(spawnPoint.position, player.position);
                }

                spawnPointDistances[spawnPoint] = totalDistance;
            }

            // Order the dictionary by distance in descending order and select the top 2 spawn points
            return spawnPointDistances.OrderByDescending(sp => sp.Value).Take(2).Select(sp => sp.Key).ToList();
        }
        
        private void OnPlayerLeft(NetworkRunner networkRunner, PlayerRef playerRef)
        {
            Debug.LogError("PlayerLeft");
            
            RefreshPuppetPlayerList();
            networkPlayerCallbacks.OnPlayerLeft(playerRef);
        }
    }
}

