using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackStage {
    Cooldown = 0, Windup = 1, Swing = 2;
}

public class AttackState : UnitActionState
{
    private AttackStage attackStage = AttackStage.Cooldown;

    private int cooldownFrames;
    private int windupFrames;
    private int swingFrames;

    public AttackState(float cooldownTime, float windupTime, float swingTime) : base() {
        this.cooldownFrames = CombatScheduler.timeToTicks(cooldownTime);
        this.windupFrames = CombatScheduler.timeToTicks(windupTime);
        this.swingFrames = CombatScheduler.timeToTicks(swingTime);
    }

    protected override void onStateEnter(UnitActionState stateFrom) {
        if(stateFrom is AttackState) {
            this.attackStage = AttackStage.Cooldown;
            this.frameLockout = cooldownFrames;
        } else {
            this.attackStage = AttackStage.Windup;
            this.
        }
    }

    protected override void onStateExit() {
        this.attackStage = AttackStage.Cooldown;
        this.frameLockout = 0;
    }

    protected abstract bool readyToSwap() {
        return this.frameLockout <= 0 && this.attackStage == AttackStage.Swing;
    }

    protected abstract void runUpdate() {
        if(this.frameLockout <= 0 && this.attackStage == AttackStage.Cooldown) {
            this.attackStage = AttackStage.Windup;
        } else if(this.frameLockout <= 0 && this.attackStage == AttackStage.Windup) {
            this.attackStage = AttackStage.Swing;
        }


    }

    private void runWindup() {

    }

    private void runAttack() {

    }

    
}
