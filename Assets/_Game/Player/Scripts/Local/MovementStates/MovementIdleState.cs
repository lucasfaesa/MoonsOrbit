using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class MovementIdleState : MovementGroundedState
{
    public MovementIdleState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
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
            stateMachine.ChangeState(context.MovementJumpState);
        
        else if(base.CheckForFalling());
            //do nothing, the check already does    
        else
            base.CheckIdleOrWalk();
        
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
