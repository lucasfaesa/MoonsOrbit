using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Helpers;
using UnityEngine;

namespace Networking
{
    public class LocalPlayerToPuppetSynchronizer : MonoBehaviour
    {
        [Header("SOs")]
        [SerializeField] private NetworkRunnerCallbacksSO networkRunnerCallbacks;
        [Header("Components")]
        [SerializeField] private Transform playerModelTransform;
        [SerializeField] private Transform gunTransform;
        [SerializeField] private Transform bulletTrailRefTransform;

        private bool _hasShotThisFrame;
        private BulletTrailNetworkData _bulletTrailNetworkData;
        
        private void OnEnable()
        {
            networkRunnerCallbacks.Input += OnInput;
        }

        private void OnDisable()
        {
            networkRunnerCallbacks.Input -= OnInput;
        }

        public void SetBulletTrailData(BulletTrailNetworkData data)
        {
            _hasShotThisFrame = true;
            _bulletTrailNetworkData = data;
        }

        private void OnInput(NetworkRunner networkRunner, NetworkInput networkInput)
        {
            var inputData = new PuppetPlayerInputData
            {
                PlayerTransformNetworkData = new PlayerTransformNetworkData(playerModelTransform.position, playerModelTransform.rotation),
                GunTransformNetworkData = new GunTransformNetworkData(gunTransform.position, gunTransform.rotation, bulletTrailRefTransform.position, bulletTrailRefTransform.rotation),
                HasShotThisFrame = _hasShotThisFrame
            };
            
            if (_hasShotThisFrame)
            {
                inputData.BulletTrailNetworkData = _bulletTrailNetworkData;
                _hasShotThisFrame = false;
            }
            
            networkInput.Set(inputData);
        }
    }
}

public struct PuppetPlayerInputData : INetworkInput
{
    public PlayerTransformNetworkData PlayerTransformNetworkData { get; set; }
    public GunTransformNetworkData GunTransformNetworkData { get; set; }
    public NetworkBool HasShotThisFrame { get; set; }
    public BulletTrailNetworkData BulletTrailNetworkData { get; set; }
}

public struct PlayerTransformNetworkData : INetworkInput
{
    public Vector3 PlayerModelVisualPos { get; set; }
    public Quaternion PlayerModelVisualRot { get; set; }
    
    public PlayerTransformNetworkData(Vector3 playerModelVisualPos, Quaternion playerModelVisualRot)
    {
        PlayerModelVisualPos = playerModelVisualPos;
        PlayerModelVisualRot = playerModelVisualRot;
    }
}

public struct GunTransformNetworkData : INetworkInput
{
    public Vector3 GunModelVisualPos { get; set; }
    public Quaternion GunModelVisualRot { get; set; }
    public Vector3 GunMuzzleRefPos { get; set; }
    public Quaternion GunMuzzleRefRot { get; set; }

    public GunTransformNetworkData(Vector3 gunModelVisualPos, Quaternion gunModelVisualRot, Vector3 gunMuzzleRefPos, Quaternion gunMuzzleRefRot)
    {
        GunModelVisualPos = gunModelVisualPos;
        GunModelVisualRot = gunModelVisualRot;
        GunMuzzleRefPos = gunMuzzleRefPos;
        GunMuzzleRefRot = gunMuzzleRefRot;
    }
}

public struct BulletTrailNetworkData : INetworkInput
{
    public Vector3 Target { get; set; }
    public NetworkBool HitSomething { get; set; }
    public Vector3 TargetNormal { get; set; }
    public Vector3 Direction { get; set; }
    public ConstantsManager.TargetType TargetType { get; set; }

    public BulletTrailNetworkData(Vector3 target, NetworkBool hit, Vector3 normal, Vector3 direction, ConstantsManager.TargetType type)
    {
        Target = target;
        HitSomething = hit;
        TargetNormal = normal;
        TargetType = type;
        Direction = direction;
    }
}
