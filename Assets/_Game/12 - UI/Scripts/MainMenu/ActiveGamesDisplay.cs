using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveGamesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playersAmount;
    [SerializeField] private TextMeshProUGUI region;
    [Space]
    [SerializeField] private Button joinButton;

    private string _roomName;
    
    public void Setup(string roomName, string playersAmount, string region)
    {
        this.roomName.text = roomName;
        _roomName = roomName;
        this.playersAmount.text = playersAmount;
        this.region.text = region;
        
        joinButton.onClick.AddListener(JoinGame);
    }

    private void OnDisable()
    {
        joinButton.onClick.RemoveListener(JoinGame);
    }

    private void JoinGame()
    {
        joinButton.interactable = false;
        NetworkManager.Instance.Connect(_roomName);
    }
}
