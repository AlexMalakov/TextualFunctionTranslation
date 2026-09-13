using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : UnitActionState
{
    private int walkFrames;
    private float rotTime; //doesn't need to be in frames it's purely cosmetic
    private HexSquare walkTo;
    private Coroutine shwalkin;
    private Coroutine shrotatin;
    private Quaternion targetRot;
    private bool rotAsigned = false;

    public WalkState(Unit u) : base(u) {
        this.walkFrames = CombatScheduler.timeToTicks(GlobalData.TimeWalk);
        this.rotTime = GlobalData.TimeTurn;
    }

    public override void onStateEnter(UnitActionState stateFrom, StateDataContainer data) {
        this.target = data.unit;
        this.walkTo = data.sq;

        if(this.walkTo != null && this.walkTo.isSquare()) {
            this.unitOwner.placeUnitOnSquare(this.walkTo, false);
            this.frameLockout = this.walkFrames;  
            shwalkin = GlobalData.Larper.FakeStartCoroutine(new CoroutineableMethod(this.walkToTarget));
            shrotatin = GlobalData.Larper.FakeStartCoroutine(new CoroutineableMethod(this.rotateToTarget));
            this.unitOwner.triggerAnimation("run");
        }
    }

    public override void onStateExit() {
        this.frameLockout = 0;

        if(this.shwalkin != null) {
            GlobalData.Larper.FakeStopCoroutine(this.shwalkin);
            this.shwalkin = null;
        }

        if(this.shrotatin != null) {
            GlobalData.Larper.FakeStopCoroutine(this.shrotatin);
            this.shrotatin = null;
        }

        if(this.rotAsigned) {
            this.unitOwner.transform.rotation = this.targetRot;
            this.rotAsigned = false;
        }

        if(this.walkTo != null && this.walkTo.isSquare()) {
            Vector3 dest = new Vector3(this.walkTo.transform.position.x, this.unitOwner.transform.position.y, this.walkTo.transform.position.z);
            this.unitOwner.transform.position = dest;
            this.walkTo = null;
        }
    }

    protected override bool readyToSwap() {
        return this.frameLockout <= 0;
    }

    protected override void runUpdate() {}

    private IEnumerator walkToTarget() {
        Vector3 startPos = this.unitOwner.transform.position;
        Vector3 dest = new Vector3(this.walkTo.transform.position.x, startPos.y, this.walkTo.transform.position.z);

        while(this.frameLockout > 0) {
            float elap = (this.walkFrames - this.frameLockout)/(float)this.walkFrames;
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
        this.rotAsigned = true;

        float elapsed = 0f;

        while(elapsed < this.rotTime) {
            this.unitOwner.transform.rotation = Quaternion.Slerp(startRot, this.targetRot, elapsed/this.rotTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.unitOwner.transform.rotation = this.targetRot;
    }
}