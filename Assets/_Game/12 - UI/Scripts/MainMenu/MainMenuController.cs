using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Networking;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
    [Space]
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private ActiveGamesDisplay activeGamesDisplay;
    [Space]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button quitButton;
    
    private List<ActiveGamesDisplay> _instantiatedActiveGamesDisplayList = new();

    private void Awake()
    {
        networkRunnerCallbacks.SessionListUpdated += OnSessionListUpdated;
    }

    private void OnEnable()
    {
        createRoomButton.onClick.AddListener(OnCreateRoom);
        quitButton.onClick.AddListener(OnQuit);
    }

    private void OnDisable()
    {
        createRoomButton.onClick.RemoveListener(OnCreateRoom);
        quitButton.onClick.RemoveListener(OnQuit);
        
        
        networkRunnerCallbacks.SessionListUpdated -= OnSessionListUpdated;
    }
    
    private void OnSessionListUpdated(NetworkRunner networkRunner, List<SessionInfo> sessionInfos)
    {
        if (_instantiatedActiveGamesDisplayList.Count > 0)
        {
            foreach (var t in _instantiatedActiveGamesDisplayList)
            {
                Destroy(t.gameObject);
            }
            
            _instantiatedActiveGamesDisplayList.Clear();
        }
            
        
        foreach (var sessionInfo in sessionInfos)
        {
            ActiveGamesDisplay instantiatedActiveGamesDisplay = Instantiate(activeGamesDisplay, scrollViewContent);
            
            instantiatedActiveGamesDisplay.Setup(sessionInfo.Name, sessionInfo.PlayerCount.ToString(), sessionInfo.Region);
            
            _instantiatedActiveGamesDisplayList.Add(instantiatedActiveGamesDisplay);
        }
    }
    
    private async void OnCreateRoom()
    {
        createRoomButton.interactable = false;

        await NetworkManager.Instance.Connect(GenerateRandomString(7));
    }
    
    string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        return new string(result);
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
