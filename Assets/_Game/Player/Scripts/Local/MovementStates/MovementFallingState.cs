
using DesignPatterns;
using UnityEngine;


public class MovementFallingState : MovementAirborneState
{

    public MovementFallingState(PlayerMovement context, StateMachine<PlayerMovement> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        //Debug.Log("<color=red>Entered falling state</color>");
    }

    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        context.HandleMovement();
        
        if (context.IsGrounded)
        {
            if (context.InputReader.Direction != Vector2.zero)
                stateMachine.ChangeState(context.MovementWalkState);
            else
                stateMachine.ChangeState(context.MovementIdleState);
        }
            
    }

    public override void PhysicsUpdate()
    {
        base.LogicUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }

}
