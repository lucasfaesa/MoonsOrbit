using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class IdleState : GroundedState
{
    public IdleState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("<color=blue>Entered Idle state </color>");
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        context.HandleMovement();
        
        if(context.InputReader.JumpStatus)
            stateMachine.ChangeState(context.JumpState);
        else
        {
            base.CheckForFalling();
            base.CheckIdleOrWalk();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    public override void Exit()
    {
        base.Exit();
    }
}
