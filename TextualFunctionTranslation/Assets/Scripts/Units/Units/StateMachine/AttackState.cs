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
        //TODO: set cooldown time and windup time based on attackspeed dynamically

        if(stateFrom is AttackState) {
            this.attackStage = AttackStage.Cooldown;
            this.runCooldown();
        } else {
            this.attackStage = AttackStage.Windup;
            StartCoroutine(this.runWindup()); //TODO: this will need to get replaced by my coroutine runner gah
        }
    }

    protected override void onStateExit() {
        this.attackStage = AttackStage.Cooldown;
        this.frameLockout = 0;
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0 && this.attackStage == AttackStage.Swing;
    }

    protected override void runUpdate() {
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

    private IEnumerator runWindup() {
        this.frameLockout = this.windupFrames;

        //look at the target

        //play the windup animation
    }

    private IEnumerator runSwing() {
        this.frameLockout = this.swingFrames;

        //deal damage

        //play animation
    }

    
}
