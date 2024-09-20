using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using DG.Tweening;
using UnityEngine;

public class CombatReloadState : State<PlayerCombatBehavior>
{
    public CombatReloadState(PlayerCombatBehavior context, StateMachine<PlayerCombatBehavior> stateMachine) : base(context, stateMachine)
    {
    }

    private bool _initalized;
    private Sequence _reloadAnimation;
    private bool _isReloading;
    
    
    public override void Enter()
    {
        base.Enter();
        Debug.Log("<color=magenta>Entered combat reload state</color>");

        context.CanAim = false;
        context.ToggleAim(false);
        
        _isReloading = true;
        
        if (!_initalized)
        {
            _reloadAnimation = DOTween.Sequence().Append(context.PlayerWeaponHolder
                .DOLocalRotate(Vector3.right * -1080f, context.PistolStats.ReloadTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack, 1f));
        }
        
        _reloadAnimation.Play().OnComplete(()=>
        {
            context.BulletsLeft = context.PistolStats.BulletsPerClip;
            
            _isReloading = false;
        });
    }

    public override void LogicUpdate()
    {
        if (_isReloading) 
            return;
        
        if(context.InputReader.ShootStatus)
            stateMachine.ChangeState(context.CombatFightState);
        else
            stateMachine.ChangeState(context.CombatIdleState);

    }

    public override void PhysicsUpdate()
    {
    }

    public override void Exit()
    {
        base.Exit();
        context.CanAim = true;
    }
}
