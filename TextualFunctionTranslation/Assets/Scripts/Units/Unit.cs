using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct StarStats {
    public float star1Stat;
    public float star2Stat;
    public float star3Stat;

    public float getStat(int starLevel) {
        switch(starLevel) {
            case 1:
                return this.star1Stat;
            case 2:
                return this.star2Stat;
            case 3:
                return this.star3Stat;
            default:
                return -1;
        }
    }
}

enum UnitState {
    Attacking, Idle
}

public abstract class Unit : MonoBehaviour
{
    [Header ("Stats")]
    [SerializeField] private StarStats health;
    [SerializeField] private StarStats damage;
    [SerializeField] private float attackSpeed; //may not needed?
    [SerializeField] private int range;
    [SerializeField] private int cost;
    [SerializeField] private Trait roleTrait;
    [SerializeField] private Trait classTrait;

    private int starLevel = 1;
    private float combatHealth;

    [Header ("Hidden stats")]
    [SerializeField] private int decisionPriority;
    [SerializeField] private int tauntAmount;

    [Header ("Objects")]
    [SerializeField] private CombatManager manager;

    private float currentHealth;
    private Unit currentTarget;

    public int getDecisionPriority() {
        return this.decisionPriority;
    }

    public bool isDead() {
        return currentHealth <= 0.0;
    }

    public int getRange() {
        return this.range;
    }

    public int getTaunt() {
        return this.tauntAmount;
    }

    public void checkUnitHasTask(CombatManager manager) {
        if(currentTarget == null || currentTarget.isDead /* || distance(this, currentTarget) > range*/) {
            this.currentTarget = manager.requestAttackTarget(this, this.currentTarget);
            this.unitAttack();
            //problem: need to wait out animation
        }
    }

    public void unitAttack() {
        //problem: if we kill a unit we skip the down time
        if(currentTarget == null || currentTarget.isDead /* || distance(this, currentTarget) > range*/) {
            //can't do anything we don't have a target
            // this.manager.requestAttackTarget(this.currentTarget);
            return;
        }

        //attack the target
        this.currentTarget.takeDamage(this.damage.getStat(this.starLevel));
        Invoke(nameOf)
    }

    public void takeDamage(float damage) {

    }

    private IENumerator attackTarget() {
        //TODO Implement
        yield break;
    }


    /*
     - ACTION (attack) BEHAVIOR:
         - start with no target
         - in order of decisionPriority, ask manager for a target
         - check all targets within it's range, chose the one with the highest priority
     - 
         - attack target while able
         - if target dies or moves 
     - 
     -  event based? 
     -  issue: how do we do order
     - 
    */
}
