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
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float timeBetweenMove;
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

    private HexSquare curPosition; //TODO: INIT ME?

    private int framesToPause = 0;

    int totalUpdatesTillNext = 0;

    private bool isMoving;
    private bool requestOverride;

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
        if(this.currentTarget == null || this.currentTarget.isDead() /* || distance(this, currentTarget) > range*/) {
            this.currentTarget = this.manager.requestAttackTarget(this, this.currentTarget);
            //problem: need to wait out animation
        }
    
        if(this.framesToPause > 0) { //i am already doing a task
            this.updateLockout();
            return;
        }
        
        //i am not doing a task but have a target, so i am going to go to that target
        if(this.currentTarget == null) {
            this.beIdle();//do nothing for a couple of frames, try again after
        }else if(this.canReachTarget()) {
            this.unitAttack();
        } else {
            this.walkTowardsTarget();
        }
    }

    public HexSquare getPosition() {
        return this.curPosition;
    }



    private void unitAttack() {
        //attack target
        this.currentTarget.takeDamage(this.damage.getStat(this.starLevel));

        //TODO: trigger animation

        this.setLockout(this.timeBetweenAttack);
    }

    //returns true if target is in range
    private bool canReachTarget() {
        if(this.currentTarget == null) {
            return false;
        }
        return HexSquare.hexDist(this.getPosition(), this.currentTarget.getPosition()) <= this.range;
    }

    //moev one square towards target
    private void walkTowardsTarget() {
        HexSquare walkTo = this.manager.requestRoute(this, this.currentTarget);
        if(walkTo.isSquare()) {
            this.getPosition().claimSquare(this, false);
            walkTo.claimSquare(this, true);
            StartCoroutine(this.moveToTarget(walkTo));
            this.setLockout(this.timeBetweenMove);
        }
    }

    private IEnumerator moveToTarget(HexSquare walkTo) {
        //TODO: play walking animation
        while(isMoving) {
            this.requestOverride = true;
            Debug.Log("waiting for an older move to abort!");
            yield return null;
        }

        this.isMoving = true;
        this.requestOverride = false;
        Vector3 startPos = transform.position;

        float duration = this.timeBetweenMove;
        float elapsed = 0f;

        while(elapsed < duration) {
            if(this.requestOverride) {
                this.isMoving = false;
                yield break;
            }

            this.transform.position = Vector3.Lerp(startPos, walkTo.transform.position, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.isMoving = false;
    }

    private void beIdle() {
        //TODO: idle animation?
        this.setLockout(.2f);
    }

    private void setLockout(float duration) {
        this.framesToPause = (int) (duration / CombatScheduler.combatTimePerTick);
    }

    private void updateLockout() {
        this.framesToPause -= 1;
    }

    public void takeDamage(float damage) {
        this.currentHealth -= damage;
        //set healthbar to be max of 0, currentHealth

        if(this.currentHealth <= 0) {
            this.onDeath();
            return;
        }
    }

    public void onDeath() {

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

