using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class MovementWalkState :  MovementGroundedState
{
    public MovementWalkState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("<color=green>Entered walk state</color>");
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        context.HandleMovement();
        
        if(context.InputReader.JumpStatus)
            stateMachine.ChangeState(context.MovementJumpState);
        
        else if(base.CheckForFalling())
            return; //do nothing, the check already does     
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
