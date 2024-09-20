using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using DG.Tweening;
using UnityEngine;

public class CombatIdleState : State<PlayerCombatBehavior>
{
    public CombatIdleState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }

    private bool _initialized;
    private Sequence _weaponSwaySequence;
    private Sequence _aimingWeaponSwaySequence;
    private Sequence _resetWeaponPosition;
    
    public override void Enter()
    {
        base.Enter();
        
        if (!_initialized)
        {
            _weaponSwaySequence = context.PlayerWeaponHolder
                .DOLocalJump(new Vector3(0.276f, -0.21f, 0.44f),
                    0.01f, 1, 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Pause().SetAutoKill(false)
                .OnPlay(()=>
                {
                    context.PlayerWeaponHolder.localPosition = context.WeaponHolderInitialPosition;
                });
        }
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
        
        AnimateWeaponSway(!context.IsAiming && context.InputReader.Direction.magnitude != 0);
    }

    private void AnimateWeaponSway(bool moving)
    {
        if (_isThisStateExiting)
            return;

        if (moving)
            _weaponSwaySequence.Play();
        else
            _weaponSwaySequence.Pause();
    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {
        base.Exit();
        
        _weaponSwaySequence.Pause();
    }
}
