using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackStage {
    Cooldown = 0, Windup = 1, Swing = 2, Cancled = -1
}

public class AttackState : UnitActionState
{
    private AttackStage attackStage = AttackStage.Cancled;

    private float cooldownTime;
    private float windupTime;
    private int swingFrames;

    private int attackSpeedCooldownFrames;
    private int attackSpeedWindupFrames;

    private int minSwingFrames;

    private Coroutine windupCoroutine;

    public AttackState(Unit u, float cooldownTime, float windupTime) : base(u) {
        this.swingFrames = CombatScheduler.timeToTicks(GlobalData.TimeSwing);
        this.cooldownTime = cooldownTime;
        this.windupTime = windupTime;

        this.minSwingFrames = CombatScheduler.timeToTicks(GlobalData.TimeMinWindup);
    }

    public override void onStateEnter(UnitActionState stateFrom, StateDataContainer data) {
        this.target = data.unit;
        float ats = Mathf.Max(data.flt, .1f);

        //be careful of the logic here
        float wind = (windupTime / Mathf.Pow(ats, .7f));
        this.attackSpeedWindupFrames = (int)Mathf.Max(minSwingFrames, CombatScheduler.timeToTicks(wind));;

        float cool = (cooldownTime / Mathf.Pow(ats, .7f));
        this.attackSpeedCooldownFrames = (int)(CombatScheduler.timeToTicks(cool));

        if(stateFrom is AttackState) {
            this.attackStage = AttackStage.Cooldown;
            this.runCooldown();
        } else {
            this.attackStage = AttackStage.Windup;
            this.windupCoroutine = GlobalData.Larper.FakeStartCoroutine(new CoroutineableMethod(this.runWindup));
        }
    }

    public override bool cancelAction() {
        base.cancelAction();
        if(this.attackStage == AttackStage.Windup || this.attackStage == AttackStage.Cooldown) {
            this.onStateExit(); //force quit the coroutine
        }

        return true;
    }

    public override void onStateExit() {
        if(this.windupCoroutine != null) {
            GlobalData.Larper.FakeStopCoroutine(this.windupCoroutine);
            Debug.Log("COROUTINE EXECUTED!!!!" + this.unitOwner.gameObject.name);
            this.windupCoroutine = null;
        }

        this.attackStage = AttackStage.Cancled;
        this.frameLockout = 0;
    }

    protected override bool readyToSwap() {
        return (this.frameLockout <= 0 && this.attackStage == AttackStage.Swing) || this.attackStage == AttackStage.Cancled;
    }

    protected override void runUpdate() {
        if(this.frameLockout <= 0 && this.attackStage == AttackStage.Cooldown) {
            this.attackStage = AttackStage.Windup;
            this.windupCoroutine = GlobalData.Larper.FakeStartCoroutine(new CoroutineableMethod(this.runWindup));
        } else if(this.frameLockout <= 0 && this.attackStage == AttackStage.Windup) {
            this.attackStage = AttackStage.Swing;
            this.runSwing();
        }
    }

    private void runCooldown() {
        this.frameLockout = this.attackSpeedCooldownFrames;
    }

    private Quaternion getTargetRot() {
        Vector3 direction = this.target.transform.position - this.unitOwner.transform.position;
        direction.y = 0f;

        return Quaternion.LookRotation(direction);
    }

    private IEnumerator runWindup() {
        this.frameLockout = this.attackSpeedWindupFrames;
        this.unitOwner.triggerAnimation("windup_attack");

        while(this.frameLockout > 0 && this.attackStage == AttackStage.Windup) {
            float ratio = (this.attackSpeedWindupFrames - this.frameLockout) / (float)this.attackSpeedWindupFrames;
            this.unitOwner.transform.rotation = Quaternion.Slerp(this.unitOwner.transform.rotation, getTargetRot(), ratio);
            
            yield return null;
        }
        
        this.unitOwner.transform.rotation = getTargetRot();
        
    }

    //cannot be canceled anymore
    private void runSwing() {
        if(!this.unitOwner.canAttackTarget()) {
            this.frameLockout = 0;
            return;
        }
        this.frameLockout = this.swingFrames;
        this.unitOwner.triggerAnimation("attack");
        this.unitOwner.attackTarget();
    }

    
}
