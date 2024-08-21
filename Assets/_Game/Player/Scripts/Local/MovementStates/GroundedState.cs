using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class GroundedState : State<PlayerMovement>
{
    public GroundedState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    protected void CheckForFalling()
    {
        if (!context.IsGrounded)
            stateMachine.ChangeState(context.FallingState);
    }
    
    protected void CheckIdleOrWalk()
    {
        if (context.InputReader.Direction == Vector2.zero)
        {
            stateMachine.ChangeState(context.IdleState);
        }
        else
        {
            stateMachine.ChangeState(context.WalkState);
        }
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
