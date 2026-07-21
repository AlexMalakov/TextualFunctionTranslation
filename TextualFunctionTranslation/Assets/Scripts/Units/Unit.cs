using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[System.Serializable] //TODO: animations outside of combat
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

// mnight want this might not want this
// public struct StateData {
//     public Unit target;
//     public float attackSpeed;

//     public StateData() {

//     }
// }

public enum BuffableStats {
    Health, Damage, AttackSpeed //idk about this one ngl but ill keep it for now
}

public abstract class Unit : MonoBehaviour
{
    [Header ("Stats")]
    [SerializeField] private StarStats health;
    [SerializeField] private StarStats damage;
    [SerializeField] private float attackWindup;
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float timeBetweenMove;
    [SerializeField] private int range;
    [SerializeField] private string unitName;

    [Header ("Traits")]
    [SerializeField] private string roleTraitName;
    [SerializeField] private string classTraitName;
    private Trait roleTrait;
    private Trait classTrait;

    private UnitBlueprint unitBlueprint;
    private int starLevel = 1;

    [Header ("Hidden stats")]
    [SerializeField] private int decisionPriority;
    [SerializeField] private int tauntAmount;

    [Header ("Objects")]
    [SerializeField] private CombatManager manager;
    [SerializeField] private Animator animator;
    [SerializeField] private CombatLog combatLog;
    private HealthBarManager hpBar;

    //[Header ("Active Stats")]
    private UnitNumericCalculator healthCalculator;
    private UnitNumericCalculator damageCalculator;
    private UnitNumericCalculator attackSpeedCalculator; //need a way to balance this?

    private float currentHealth;

    private Unit currentTarget;
    private HexSquare curPosition; //TODO: INIT ME?

    private int framesToPause = 0;
    private bool isMoving;
    private bool requestOverride;

    public Unit setBlueprint(UnitBlueprint blueprint, CombatManager manager) {
        this.manager = manager;
        this.unitBlueprint = blueprint;
        return this;
    }

    public int getDecisionPriority() {
        return this.decisionPriority;
    }

    public bool isDead() {
        return currentHealth <= 0.0;
    }

    public int getRange() {
        return this.range;
    }

    public string getName() {
        return this.unitName;
    }

    public int getTaunt() {
        return this.tauntAmount;
    }

    public int getCost() {
        return this.unitBlueprint.unitCost;
    }

    public Trait getRoleTrait() {
        if(!this.roleTrait) { //this unit's trait is always known. It however cannot be retrieved until the factory is done. So this method can't be called during 'AWAKE'
            this.roleTrait = TraitFactory.getTrait(this.roleTraitName, this);
        }
        return this.roleTrait;
    }

    public Trait getClassTrait() {
        if(!this.classTrait) {
            this.classTrait = TraitFactory.getTrait(this.classTraitName, this);
        }
        return this.classTrait;
    }

    public virtual void checkUnitHasTask(CombatManager manager) {
        // if(this.framesToPause == 0) {
            // Debug.Log("I AM search me " + gameObject.name + " SOMEHOW AT THE START WITH ZERO? " + this.framesToPause);
        // }

        if(this.isDead()) {
            return;
        }

        if(this.currentTarget == null || this.currentTarget.isDead() /* || distance(this, currentTarget) > range*/) {
            this.currentTarget = this.manager.requestAttackTarget(this, this.currentTarget);
            //problem: need to wait out animation
        }
    
        if(this.framesToPause > 0) { //i am already doing a task
            Debug.Log("I AM AKREADY MOVING! hi " + gameObject.name);
            this.updateLockout();
            return;
        }
        
        //i am not doing a task but have a target, so i am going to go to that target
        if(this.currentTarget == null) {
            Debug.Log("I AM DOING NOTHING! hi " + gameObject.name);
            Debug.Log("IDLE!");
            this.beIdle();//do nothing for a couple of frames, try again after
        }else if(this.canReachTarget()) {
            Debug.Log("I AM DOING ATTACKING! hi " + gameObject.name);
            Debug.Log("ATTACK!");
            StartCoroutine(this.unitAttackWindup());
        } else {
            Debug.Log("I AM DOING WALKING! hi " + gameObject.name);
            Debug.Log("WALK!");
            this.walkTowardsTarget();
        }
    }

    public HexSquare getPosition() {
        return this.curPosition;
    }

    public void initUnit(HexSquare square, HealthBarManager hpBar, Transform hpCanvas) {
        this.hpBar = hpBar;
        this.healthCalculator = new UnitNumericCalculator(this.health.getStat(this.starLevel));
        this.damageCalculator = new UnitNumericCalculator(this.damage.getStat(this.starLevel));
        this.attackSpeedCalculator = new UnitNumericCalculator(1.0f); //TODO: make this stat real

        this.currentHealth = this.healthCalculator.calculate();

        this.currentTarget = null;

        //TODO: this wont reflect the post buffed value
        this.hpBar.init(this.currentHealth, this.transform, hpCanvas);
        this.placeUnitOnSquare(square);
    }


    public void onCombatEnd() {
        this.healthCalculator.wipe();
        this.damageCalculator.wipe();
        this.attackSpeedCalculator.wipe();
    }

    public void placeUnitOnSquare(HexSquare square) {
        if(this.curPosition && this.curPosition.isSquare()) {
            this.curPosition.claimSquare(this, false);
        }
        this.curPosition = square;
        square.claimSquare(this, true);
        this.transform.position = new Vector3(square.transform.position.x, this.transform.position.y, square.transform.position.z);
    }

    public void placeUnitOnBench() {
        this.unclaimSquare();
    }


    private IEnumerator unitAttackWindup() {
        //attack target
        Vector3 lookAt = new Vector3(this.currentTarget.transform.position.x, this.transform.position.y, this.currentTarget.transform.position.z);
        transform.LookAt(lookAt);
        // this.currentTarget.takeDamage(this.damageCalculator.calculate(), this);

        this.animator.SetTrigger("Attack");

        this.setLockout(this.timeBetweenAttack);
        yield return new WaitForSeconds(this.attackWindup);
        if(!this.isDead()) {
            attackTarget();
        }
    }

    public void attackTarget() {
        this.currentTarget.takeDamage(this.damageCalculator.calculate(), this);
        this.combatLog.logEvent(new CombatEvent(this, this.currentTarget, CombatEventType.Attack));
        this.roleTrait.onUnitAttack(this, this.currentTarget);
        this.classTrait.onUnitAttack(this, this.currentTarget);
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
            this.setLockout(this.timeBetweenMove);
            StartCoroutine(this.moveToTarget(walkTo, this.framesToPause));
        }
    }

    public void triggerAnimation(string trigger) {
        this.animator.SetTrigger(trigger);
    }

    private IEnumerator moveToTarget(HexSquare walkTo, int duration) {
        this.getPosition().claimSquare(this, false);
        walkTo.claimSquare(this, true);
        this.curPosition = walkTo;
        Vector3 startPos = transform.position;
        Vector3 dest = new Vector3(walkTo.transform.position.x, startPos.y, walkTo.transform.position.z);

        while(isMoving) { //TODO: Why do I happen? Should it be possible?
            this.requestOverride = true;
            Debug.Log("waiting for an older move to abort! " + gameObject.name);
            yield return null;
        }

        this.animator.SetTrigger("Walk");

        Vector3 lookAt = new Vector3(walkTo.transform.position.x, this.transform.position.y, walkTo.transform.position.z);
        transform.LookAt(lookAt);
        this.isMoving = true;
        this.requestOverride = false;

        int prevFramesToPause = this.framesToPause; //in case it jumps back up somehow
        while(this.framesToPause > 0) {
            // Debug.Log("I AM search me " + gameObject.name + " AND IM INSIDE THE WHILE LOOP " + this.framesToPause);
            if(this.framesToPause > prevFramesToPause) {
                // Debug.Log("I AM search me " + gameObject.name + " Frames have been increased before we hit 0!!!!");
                this.isMoving = false;
                yield break;
            }

            if(this.requestOverride) {
                // Debug.Log("I AM search me " + gameObject.name + " gpt canceleledddf!!!!");
                this.isMoving = false;
                yield break;
            }
            
            this.transform.position = Vector3.Lerp(startPos, dest, (duration - this.framesToPause)/(float)duration);
            prevFramesToPause = this.framesToPause;
            yield return null;
        }

        // Debug.Log("I AM search me " + gameObject.name + " AND I HAVE FINISHED A MOVE TO " + walkTo.toString());

        this.isMoving = false;
    }

    private void beIdle() {
        this.animator.SetTrigger("Idle");
        this.setLockout(.2f);
    }

    public void setLockout(float duration) {
        this.framesToPause = (int) (duration / CombatScheduler.combatTimePerTick);
        this.currentState.update(this.framesToPause);
    }

    private void updateLockout() {
        this.framesToPause -= 1;

        this.currentState.update(this.framesToPause);
    }

    public void takeDamage(float in_damage, Unit damage_from) {
        if(this.isDead()) {
            Debug.Log("HITTING A DEAD!");
            return;
        }

        this.currentHealth -= in_damage;
        this.hpBar.updateDamage(in_damage);

        if(this.currentHealth <= 0) {
            this.combatLog.logEvent(new CombatEvent(this, damage_from, CombatEventType.Death));
            this.combatLog.logEvent(new CombatEvent(damage_from, this, CombatEventType.Kill));

            damage_from.getRoleTrait().onEnemyDefeat(damage_from, this);
            damage_from.getClassTrait().onEnemyDefeat(damage_from, this);
            this.getRoleTrait().onUnitDeath(this, damage_from);
            this.getClassTrait().onUnitDeath(this, damage_from);

            this.onDeath();
            return;
        }
    }

    public void heal(float in_healing) {
        float preHealth = this.currentHealth;
        this.currentHealth += in_healing;

        if(this.currentHealth >= this.healthCalculator.calculate()) {
            this.currentHealth = this.healthCalculator.calculate();
        }

        this.hpBar.updateDamage(preHealth - this.currentHealth);
    }

    //this does not give a unit a new square use carefully!
    public void unclaimSquare() {
        if(this.curPosition == null || !this.curPosition.isSquare()) {
            return;
        } 
        this.curPosition.claimSquare(this, false);
        this.curPosition = null; //should prob be a null square but oh well
    }

    public void onDeath() {
        //play dying animatiom?

        this.manager.onUnitDeath(this);
        this.curPosition.claimSquare(this, false);

        //careful that this isn't called more than once
        Invoke(nameof(die), .3f);
    }

    private void die() {
        this.hpBar.onDeath();
        this.gameObject.SetActive(false);
    }

    public void mergeUnit() {
        this.starLevel += 1;
    }

    public int getMergeLevel() {
        return this.starLevel;
    }

    public void buffStat(BuffableStats stat, Operation operation, Unit source) {
        this.combatLog.logEvent(new CombatEvent(source, this, CombatEventType.Buff));
        switch(stat) {
            case BuffableStats.Health:
                this.healthCalculator.addOperation(operation);
                break;
            case BuffableStats.Damage:
                this.damageCalculator.addOperation(operation);
                break;
            case BuffableStats.AttackSpeed:
                this.attackSpeedCalculator.addOperation(operation);
                break;
        }
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

