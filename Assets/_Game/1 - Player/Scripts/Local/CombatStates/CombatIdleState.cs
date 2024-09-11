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
        if (context.InputReader.ShootStatus)
        {
            if (context.BulletsLeft > 0)
                stateMachine.ChangeState(context.CombatFightState); // Transition to CombatFightState if bullets are available
            else
                stateMachine.ChangeState(context.CombatReloadState); // Transition to CombatReloadState if no bullets are left
        }
        else if (context.InputReader.ReloadStatus && context.BulletsLeft < context.PistolStats.BulletsPerClip)
        {
            stateMachine.ChangeState(context.CombatReloadState);
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }
}
