using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Rendering;

public class MovementGroundedState : State<PlayerMovement>
{
    public MovementGroundedState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    protected bool CheckForFalling()
    {
        if (!context.IsGrounded)
            stateMachine.ChangeState(context.MovementFallingState);
        
        return !context.IsGrounded;
    }
    
    protected void CheckIdleOrWalk()
    {
        if (context.InputReader.Direction == Vector2.zero)
        {
            stateMachine.ChangeState(context.MovementIdleState);
        }
        else
        {
            stateMachine.ChangeState(context.MovementWalkState);
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
        base.Exit();
    }
}
