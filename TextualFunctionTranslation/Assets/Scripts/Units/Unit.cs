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
    [SerializeField] private TraitNames roleTraitName;
    [SerializeField] private TraitNames classTraitName;
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
            this.roleTrait = TraitFactory.getTrait(this.roleTraitName);
        }
        return this.roleTrait;
    }

    public Trait getClassTrait() {
        if(!this.classTrait) {
            this.classTrait = TraitFactory.getTrait(this.classTraitName);
        }
        return this.classTrait;
    }

    public virtual void checkUnitHasTask(CombatManager manager) {
        if(this.isDead()) {
            return;
        }

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
            Debug.Log("IDLE!");
            this.beIdle();//do nothing for a couple of frames, try again after
        }else if(this.canReachTarget()) {
            Debug.Log("ATTACK!");
            StartCoroutine(this.unitAttack());
        } else {
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
        //TODO: init attack speed calculator

        this.currentHealth = this.healthCalculator.calculate();

        this.currentTarget = null;

        //TODO: this wont reflect the post buffed value
        this.hpBar.init(this.currentHealth, this.transform, hpCanvas);
        this.placeUnitOnSquare(square);
    }


    public void onCombatEnd() {
        //TODO: reset all buffs
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


    private IEnumerator unitAttack() {
        //attack target
        Vector3 lookAt = new Vector3(this.currentTarget.transform.position.x, this.transform.position.y, this.currentTarget.transform.position.z);
        transform.LookAt(lookAt);
        // this.currentTarget.takeDamage(this.damageCalculator.calculate());

        //TODO: trigger animation
        this.animator.SetTrigger("Attack");

        this.setLockout(this.timeBetweenAttack);
        yield return new WaitForSeconds(this.attackWindup);
        if(!this.isDead()) {
            this.currentTarget.takeDamage(this.damageCalculator.calculate());
            this.roleTrait.onUnitAttack(this, this.currentTarget);
            this.classTrait.onUnitAttack(this, this.currentTarget);
        }
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
        // Debug.Log(walkTo.toString() + "<--- walkTo" + this.curPosition.toString() + "<----- walk from");
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

        this.animator.SetTrigger("Walk");

        this.curPosition = walkTo;
        Vector3 lookAt = new Vector3(walkTo.transform.position.x, this.transform.position.y, walkTo.transform.position.z);
        transform.LookAt(lookAt);
        this.isMoving = true;
        this.requestOverride = false;
        Vector3 startPos = transform.position;
        Vector3 dest = new Vector3(walkTo.transform.position.x, startPos.y, walkTo.transform.position.z);

        float duration = this.timeBetweenMove;
        float elapsed = 0f;

        while(elapsed < duration) {
            if(this.requestOverride) {
                this.isMoving = false;
                yield break;
            }

            this.transform.position = Vector3.Lerp(startPos, dest, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.isMoving = false;
    }

    private void beIdle() {
        this.animator.SetTrigger("Idle");
        this.setLockout(.2f);
    }

    private void setLockout(float duration) {
        this.framesToPause = (int) (duration / CombatScheduler.combatTimePerTick);
    }

    private void updateLockout() {
        this.framesToPause -= 1;
    }

    public void takeDamage(float in_damage) {
        if(this.isDead()) {
            Debug.Log("HITTING A DEAD!");
            return;
        }

        this.currentHealth -= in_damage;
        this.hpBar.updateDamage(in_damage);

        if(this.currentHealth <= 0) {
            this.onDeath();
            return;
        }
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

    public void buffStat(BuffableStats stat, Operation operation) {
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

