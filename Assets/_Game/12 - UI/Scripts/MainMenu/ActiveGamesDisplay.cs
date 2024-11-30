using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveGamesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI playersAmount;
    [SerializeField] private Button joinButton;

    private void Setup(string roomName, string playersAmount)
    {
        this.roomName.text = roomName;
        this.playersAmount.text = playersAmount;
        
        //joinButton.onClick.AddListener();
    }

    private void OnDisable()
    {
        //joinButton.onClick.RemoveListener();
    }
}
