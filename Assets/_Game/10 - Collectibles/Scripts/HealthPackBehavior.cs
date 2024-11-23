using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class HealthPackBehavior : NetworkBehaviour
{

    [SerializeField] private NetworkObject networkObject;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject content;
    
    private Sequence _healthPackSequence;

    public override void Spawned()
    {
        base.Spawned();
        Animate();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LocalPlayer"))
        {
            other.GetComponent<PlayerDamageable>().RegenerateHealth();

            DespawnRPC();
        }
    }

    private void Animate()
    {
        content.transform.DOLocalMoveY(0.126f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        content.transform.DOLocalRotate(new Vector3(0, 360, 0), 4f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void DespawnRPC()
    {
        Runner.Despawn(networkObject);
    }
    
}
