using System.Collections;
using System.Collections.Generic;
using Fusion;
using LocalPlayer;
using TMPro;
using UnityEngine;

public class NetworkedNameDisplay : NetworkBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private TextMeshPro nameText;
    
    [Networked, OnChangedRender(nameof(UpdatePlayerName))] private string PlayerName { get; set; }
    
    
    public override void Spawned()
    {
        base.Spawned();
        
        if(HasInputAuthority)
            PlayerName = playerStats.PlayerName;

        UpdatePlayerName();
    }

    private void UpdatePlayerName()
    {
        nameText.text = PlayerName;
    }

    private void Update()
    {
        if(Camera.main != null)
            this.transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
}
