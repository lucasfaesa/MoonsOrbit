using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PuppetPlayerToggler : NetworkBehaviour
{
    [SerializeField] private List<GameObject> objectsToDeactivateLocally;

    private bool IsLocalNetworkRig => Object.HasStateAuthority;

    public override void Spawned()
    {
        base.Spawned();

        if (IsLocalNetworkRig)
        {
            DeactivateAll();
        }
        else
            Debug.Log("This is a client object");
    }

    private void DeactivateAll()
    {
        objectsToDeactivateLocally.ForEach(x=>x.SetActive(false));
    }
}
