using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : Unit
{
    [Header ("randomizer dets")]
    [SerializeField] private int minRange;
    [SerializeField] private int maxRange;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;

    [SerializeField] private int randomSeed;

    private int currentRange = 1;

    private System.Random rng;

    public override float calcDamage() {
        if(this.rng == null) {
            this.rng = new System.Random(randomSeed);
        }

        return this.damageCalculator.calculateFromN(this.minDamage + (float)this.rng.NextDouble() * (this.maxDamage - this.minDamage));
    }

    public override int getRange() {
        return this.currentRange;
    }

    public override void triggerUpdate(CombatManager manager) {
        if(this.rng == null) {
            this.rng = new System.Random(randomSeed);
        }

        if(!this.isDead() && this.currentState != null && (this.currentTarget == null || this.currentTarget.isDead())) {
            this.currentTarget = this.manager.getRandomAttackTarget(this.rng);
            if(this.currentState.cancelAction()) {
                StateDataContainer stateData = new StateDataContainer(this.currentTarget, null, .25f, false);
                this.Idle.onStateEnter(this.currentState, stateData);
                this.currentState = this.Idle;
            }
        }

        base.triggerUpdate(manager);
    }

    public override void requestStateSwap() {
        if(this.currentState is AttackState) {
            if(this.rng == null) {
                this.rng = new System.Random(randomSeed);
            }

            this.currentTarget = this.manager.getRandomAttackTarget(this.rng);
            this.currentRange = rng.Next(this.minRange, this.maxRange+1);
            this.ac.setAttackType(this.currentRange - 1);
        }

        base.requestStateSwap();
    }

    public override void onCombatEnd() {
        base.onCombatEnd();
        this.rng = null;//force it to be reset
    }

    protected override void reassignTargetAndWalk(StateDataContainer stateData) {
        //we don't try to get a closer target we just walk :)
        stateData.sq = this.manager.requestRoute(this, this.currentTarget);

        this.walkOrWait(stateData);
    }

}
