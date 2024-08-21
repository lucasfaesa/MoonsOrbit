using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class AirborneState : State<PlayerMovement>
{
    public AirborneState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }
    
    public override void Enter()
    {
        
    }

    public override void LogicUpdate()
    {
        
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void Exit()
    {
        
    }
}
