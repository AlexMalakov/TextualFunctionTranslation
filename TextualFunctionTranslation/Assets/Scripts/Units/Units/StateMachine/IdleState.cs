using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : UnitActionState
{
    private int idleFrames;
    private bool isStun;
    private float timeStunned;

    public IdleState(float timeIdle) : base() {
        this.idleFrames = CombatScheduler.timeToTicks(timeIdle);
        this.isStun = false;
    }

    public void stun(float timeStunned) {
        this.timeStunned = timeStunned;
        this.isStun = true;
    }

    protected override void onStateEnter(UnitActionState stateFrom, Unit target) {
        if(this.isStun) {
            //TODO: stun particles
            this.frameLockout = CombatScheduler.timeToTicks(timeStunned);
        } else {
            this.frameLockout = this.idleFrames;
            this.unitOwner.triggerAnimation("idle");
        }
    }

    protected override void onStateExit() {
        this.frameLockout = 0;
        this.timeStunned = 0;
        this.isStun = false;
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0;
    }

    protected override void runUpdate() {}
}
