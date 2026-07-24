using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackStage {
    Cooldown = 0, Windup = 1, Swing = 2, Cancled = -1
}

public class AttackState : UnitActionState
{
    private AttackStage attackStage = AttackStage.Cancled;

    private int cooldownFrames;
    private int windupFrames;
    private int swingFrames;

    private int attackSpeedCooldownFrames;
    private int attackSpeedWindupFrames;

    private int minSwingFrames;

    private Coroutine windupCoroutine;

    public AttackState(float cooldownTime, float windupTime) : base() {
        this.cooldownFrames = CombatScheduler.timeToTicks(cooldownTime);
        this.windupFrames = CombatScheduler.timeToTicks(windupTime);
        this.swingFrames = CombatScheduler.timeToTicks(GlobalData.TimeSwing);

        this.minSwingFrames = CombatScheduler.timeToTicks(GlobalData.TimeMinWindup);
    }

    public override void onStateEnter(UnitActionState stateFrom, StateDataContainer data) {
        this.target = data.unit;
        float ats = data.flt;

        //be careful of the logic here
        float wind = (windupFrames / (ats + windupFrames));
        this.attackSpeedWindupFrames = (int)Mathf.Max(minSwingFrames, windupFrames * wind);

        float cool = (cooldownFrames / (ats + cooldownFrames));
        this.attackSpeedCooldownFrames = (int)(cooldownFrames * cool);

        if(stateFrom is AttackState) {
            this.attackStage = AttackStage.Cooldown;
            this.runCooldown();
        } else {
            this.attackStage = AttackStage.Windup;
            this.windupCoroutine = StartCoroutine(this.runWindup()); //TODO: this will need to get replaced by my coroutine runner gah
        }
    }

    public override bool cancelAction() {
        if(this.attackStage == AttackStage.Windup || this.attackStage == AttackStage.Cooldown) {
            this.onStateExit(); //force quit the coroutine
            //TODO: stun particles?
        }

        return true;
    }

    public override void onStateExit() {
        if(this.windupCoroutine != null) {
            StopCoroutine(this.windupCoroutine);
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
            StartCoroutine(this.runWindup());
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
        this.unitOwner.triggerAnimation("windup");

        while(this.frameLockout > 0 && this.attackStage == AttackStage.Windup) {
            //TODO: check if attack is cancel
            float ratio = (this.attackSpeedWindupFrames - this.frameLockout) / (float)this.attackSpeedWindupFrames;
            this.unitOwner.transform.rotation = Quaternion.Slerp(this.unitOwner.transform.rotation, getTargetRot(), ratio);
            yield return null;
        }
        
        this.unitOwner.transform.rotation = getTargetRot();
        
    }

    //cannot be canceled anymore
    private void runSwing() {
        this.frameLockout = this.swingFrames;
        this.unitOwner.triggerAnimation("swing");
        this.unitOwner.attackTarget();
    }

    
}
