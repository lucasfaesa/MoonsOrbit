using Fusion;
using NaughtyAttributes;
using Networking;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/Weapon/WeaponStats")]
public class WeaponStatsSO : ScriptableObject
{
    [field:SerializeField] public float Damage { get; private set; }
    [field:SerializeField] public int BulletsPerClip { get; private set; }
    [field:SerializeField] public float DelayBetweenShots { get; private set; }
    [field:SerializeField] public float ReloadTime { get; private set; }
    
    [field:SerializeField] public bool HasBulletSpread { get; private set; }
    [field:ShowIf("HasBulletSpread")] [field: SerializeField] public Vector3 BulletSpreadVariance { get; private set; } = new Vector3(0.1f, 0.1f, 0.1f);
    
    [field: SerializeField] public LayerMask Mask { get; private set; }
    


}
