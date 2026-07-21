using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : UnitActionState
{
    private int walkFrames;
    private HexSquare walkTo;
    private Coroutine shwalkin;
    private Coroutine shrotatin;
    private Quaternion targetRot;

    public WalkState(float walkTine) : base() {
        this.walkFrames = CombatScheduler.timeToTicks(walkTine);
    }

    protected override void onStateEnter(UnitActionState stateFrom, Unit target) {
        this.target = target;

        this.walkTo = this.unitOwner.manager.requestRoute(this.unitOwner, this.target);
        if(walkTo != null && this.walkTo.isSquare()) {
            this.unitOwner.walkingTo(this.walkTo); //should unclaim this.getPosition(), then shouold claim walkTo, then should set curPos = walkTo
            this.frameLockout = this.walkFrames;
            shwalkin = StartCoroutine(walkToTarget());
            shrotatin = StartCoroutine(rotateToTarget()); //TODO: stupid chud coroutine
        }
    }

    protected override void onStateExit() {
        this.frameLockout = 0;

        if(this.shwalkin != null) {
            StopCoroutine(this.shwalkin);
            this.shwalkin = null;
        }

        if(this.shrotatin != null) {
            StopCoroutine(this.shrotatin);
            this.shrotatin = null;
        }

        if(this.targetRot != null) {
            this.unitOwner.transform.LookAt(this.targetRot);
            this.targetRot = null;
        }

        if(this.walkTo != null && this.walkTo.isSquare()) {
            Vector3 dest = new Vector3(this.walkTo.transform.position.x, this.unitOwner.transform.position.y, this.walkTo.transform.position.z);
            this.unitOwner.transform.position = dest;
            this.this.walkTo = null;
        }
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0;
    }

    protected override void runUpdate() {}


    private IEnumerator walkToTarget() {
        Vector3 startPos = this.unitOwner.transform.position;
        Vector3 dest = new Vector3(this.walkTo.transform.position.x, startPos.y, this.walkTo.transform.position.z);

        //TODO: walk animation
        while(this.frameLockout > 0) {
            float elap = (this.walkFrames - this.framesToPause)/(float)this.walkFrames;
            this.unitOwner.transform.position = Vector3.Lerp(startPos, dest, elap);
            yield return null;
        }

        this.unitOwner.transform.position = dest;
    }

    private IEnumerator rotateToTarget() {
        Quaternion startRot = this.unitOwner.transform.rotation;
        
        Vector3 direction = this.walkTo.transform.position - this.unitOwner.transform.position;
        direction.y = 0f;
        
        this.targetRot = Quaternion.LookRotation(direction);

        while(this.frameLockout > 0) {
            float elap = (this.rotFrames - this.frameLockout)/(float)this.rotFrames;

            this.unitOwner.transform.rotation = Quaternion.Slerp(startRot, this.targetRot, elap);
            yield return null;
        }

        this.unitOwner.transform.rotation = this.targetRot;
    }
}