using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : UnitActionState
{
    private int idleFrames;

    public IdleState(Unit u) : base(u) {
        this.idleFrames = CombatScheduler.timeToTicks(GlobalData.TimeIdle);
    }


    public override void onStateEnter(UnitActionState stateFrom, StateDataContainer data) {
        if(data.flt > 0f) {
            //TODO: stun particles
            this.frameLockout = CombatScheduler.timeToTicks(data.flt);
        } else {
            this.frameLockout = this.idleFrames;
            this.unitOwner.triggerAnimation("idle");
        }
    }

    public override void onStateExit() {
        this.frameLockout = 0;
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0;
    }

    protected override void runUpdate() {}
}
