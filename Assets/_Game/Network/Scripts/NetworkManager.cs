using System.Collections;
using System.Threading.Tasks;
using Fusion;
using Helpers;
using UnityEngine;

namespace Networking
{
    public class NetworkManager : SingletonBase<NetworkManager>
    {
        [SerializeField] private NetworkRunner runnerPrefab;
        [SerializeField] private NetworkSceneManagerDefault networkSceneManagerDefault;
        
        public NetworkRunner NetworkRunner { get; private set; }
        
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        void Start()
        {
            CreateRunner();
        }

        private void CreateRunner()
        {
            NetworkRunner = Instantiate(runnerPrefab, transform).GetComponent<NetworkRunner>();
        }

        private async Task Connect()
        {
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = "TestSession",
                SceneManager = networkSceneManagerDefault
            };

            var result = await NetworkRunner.StartGame(args);

            if (result.Ok)
                Debug.Log("StartGame successful");
            else
                Debug.LogError($"Error: {result.ErrorMessage}");
            
            //eventsChannel.OnConnectedToRoom();
        }
        
    }
}

