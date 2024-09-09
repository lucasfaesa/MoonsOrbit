using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class CombatIdleState : State<PlayerCombatBehavior>
{
    public CombatIdleState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("<color=blue>Entered combat idle state</color>");
    }

    public override void LogicUpdate()
    {
        if(context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatFightState);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }
}
