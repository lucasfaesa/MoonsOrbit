using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class MovementJumpState : MovementAirborneState
{
    public MovementJumpState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        //Debug.Log("<color=magenta>Entered jump state</color>");
        
        base.Enter();
        Jump();
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        context.HandleMovement();
        
        if(context.CharacterController.velocity.y < 0)
            stateMachine.ChangeState(context.MovementFallingState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }

    private void Jump()
    {
        var newVel = context.Velocity;
        newVel.y += context.PlayerStats.JumpImpulse;
        context.Velocity = newVel;
    }
}
