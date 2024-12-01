using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LocalPlayer
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/Player/PlayerStats")]
    public class PlayerStatsSO : ScriptableObject
    {
        [field:SerializeField] public string PlayerName { get; set; }
        [field:SerializeField] public float Acceleration { get; set; } = 10;
        [field:SerializeField] public float Braking { get; set; } = 10;
        [field:SerializeField] public float MaxSpeed { get; set; } = 2;
        [field:SerializeField] public float RotationSpeed { get; set; } = 15;
        [field:SerializeField] public float JumpImpulse { get; set; } = 8f;
        [field:SerializeField] public float Gravity { get; set; } = -20f;
        [field:Space]
        [field:SerializeField] public Vector2 MouseSensitivity { get; set; } = new Vector2(2f,2f);
    } 
}

