using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private ActiveGamesDisplay activeGamesDisplay;
    [Space]
    [SerializeField] private Button createRoomButton;
    
    
    private void OnEnable()
    {
        createRoomButton.onClick.AddListener(OnCreateRoom);
    }

    private void OnDisable()
    {
        createRoomButton.onClick.RemoveListener(OnCreateRoom);
    }

    private async void OnCreateRoom()
    {
        createRoomButton.interactable = false;

        await NetworkManager.Instance.Connect();
    }
}
