using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct StarStats {
    public float star1Stat;
    public float star2Stat;
    public float star3Stat;
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

    public void checkUnitHasTask(CombatManager manager) {
        if(currentTarget == null || currentTarget.isDead /* || distance(this, currentTarget) > range*/) {
            manager.requestAttackTarget(this, this.currentTarget)
        }

        //else does nothing
    }

    public void unitAttack() {
        if(currentTarget == null || currentTarget.isDead /* || distance(this, currentTarget) > range*/) {
            //can't do anything we don't have a target
            // this.manager.requestAttackTarget(this.currentTarget);
            return;
        }

        //attack the target
        StartCoroutine(attackTarget)
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
