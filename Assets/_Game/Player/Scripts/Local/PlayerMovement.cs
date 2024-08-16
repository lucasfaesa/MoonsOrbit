using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput<PlayerInputData>(out var inputData))
        {
            transform.Translate(inputData.Direction * Runner.DeltaTime * speed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
