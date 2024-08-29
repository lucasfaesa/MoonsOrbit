using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletBehavior : NetworkBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Rigidbody bulletRigidbody;

    public override void Spawned()
    {
        base.Spawned();
        bulletRigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
    }
}
