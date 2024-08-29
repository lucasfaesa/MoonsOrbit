using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuggerSO", menuName = "ScriptableObjects/Debug/DebuggerSO")]
public class DebuggerSO : ScriptableObject
{
    [SerializeField] private bool forceOffline;

    public bool ForceOffline
    {
        get
        {
            #if UNITY_EDITOR
                return forceOffline;
            #else
                return false;
            #endif
        }
        set
        {
            #if UNITY_EDITOR
                forceOffline = value;
            #endif
        }
    }
}
