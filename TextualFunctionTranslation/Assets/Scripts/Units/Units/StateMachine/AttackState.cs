using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackStage {
    Cooldown = 0, Windup = 1, Swing = 2, Cancled = -1;
}

public class AttackState : UnitActionState
{
    private AttackStage attackStage = AttackStage.Cancled;

    private int cooldownFrames;
    private int windupFrames;
    private int swingFrames;

    public AttackState(float cooldownTime, float windupTime, float swingTime) : base() {
        this.cooldownFrames = CombatScheduler.timeToTicks(cooldownTime);
        this.windupFrames = CombatScheduler.timeToTicks(windupTime);
        this.swingFrames = CombatScheduler.timeToTicks(swingTime);
    }

    protected override void onStateEnter(UnitActionState stateFrom, Unit target) {
        //TODO: set cooldown time and windup time based on attackspeed dynamically
        this.target = target;

        if(stateFrom is AttackState) {
            this.attackStage = AttackStage.Cooldown;
            this.runCooldown();
        } else {
            this.attackStage = AttackStage.Windup;
            StartCoroutine(this.runWindup()); //TODO: this will need to get replaced by my coroutine runner gah
        }
    }

    protected override void onStateExit() {
        this.attackStage = AttackStage.Cancled;
        this.frameLockout = 0;
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0 && this.attackStage == AttackStage.Swing;
    }

    protected override void runUpdate() { //TODO: on attack canceled handle it (insta swap to stun?)
        if(this.frameLockout <= 0 && this.attackStage == AttackStage.Cooldown) {
            this.attackStage = AttackStage.Windup;
            StartCoroutine(this.runWindup());
        } else if(this.frameLockout <= 0 && this.attackStage == AttackStage.Windup) {
            this.attackStage = AttackStage.Swing;
            StartCoroutine(this.runSwing());
        }
    }

    private void runCooldown() {
        this.frameLockout = this.cooldownFrames;
    }

    private Quaternion getTargetRot() {
        Vector3 direction = this.target.transform.position - this.unitOwner.transform.position;
        direction.y = 0f;

        return Quaternion.LookRotation(direction);
    }

    private IEnumerator runWindup() {
        this.frameLockout = this.windupFrames;
        this.unitOwner.triggerAnimation("windup");

        while(this.frameLockout > 0 && this.attackStage == AttackStage.Windup) {
            //TODO: check if attack is cancel
            float ratio = (this.windupFrames - this.frameLockout) / (float)this.windupFrames;
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
