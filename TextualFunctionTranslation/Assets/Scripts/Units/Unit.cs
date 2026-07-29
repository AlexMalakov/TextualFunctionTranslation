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

public enum BuffableStats {
    Health, Damage, AttackSpeed
}

public struct StateDataContainer {
    public Unit unit;
    public HexSquare sq;
    public float flt;

    public StateDataContainer(Unit unit, HexSquare sq, float flt) {
        this.unit = unit; this.sq = sq; this.flt = flt;
    }
}

public abstract class Unit : MonoBehaviour
{
    [Header ("Stats")]
    [SerializeField] private StarStats health;
    [SerializeField] private StarStats damage;
    [SerializeField] private int attackSpeed = 1; //IDEA: attack speed is calculated as total attack frames / (total attack frames + attack speed)
    [SerializeField] private int range;
    [SerializeField] private string unitName;

    [Header ("State times")]
    [SerializeField] private float attackWindupTime;
    [SerializeField] private float attackCooldownTime;
    
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
    private HexSquare curPosition;

    //states
    UnitActionState Walk;
    UnitActionState Idle;
    UnitActionState Attack;
    private UnitActionState currentState;

    public void Start() {
        this.Walk = new WalkState(this);
        this.Idle = new IdleState(this);
        this.Attack = new AttackState(this, this.attackCooldownTime, this.attackWindupTime);
    }

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

    public HexSquare getPosition() {
        return this.curPosition;
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

    public void initUnit(HexSquare square, HealthBarManager hpBar, Transform hpCanvas) { //NOTE: this is called both at the start of prep and end of prep
        if(this.hpBar != null) {
            this.hpBar.onDeath();
        }
        this.hpBar = hpBar;
        this.healthCalculator = new UnitNumericCalculator(this.health.getStat(this.starLevel));
        this.damageCalculator = new UnitNumericCalculator(this.damage.getStat(this.starLevel));
        this.attackSpeedCalculator = new UnitNumericCalculator((float)this.attackSpeed);

        this.currentHealth = this.healthCalculator.calculate();

        this.currentTarget = null;

        this.hpBar.init(this.currentHealth, this.transform, hpCanvas);
        this.placeUnitOnSquare(square, true);
    }

    public virtual void triggerUpdate(CombatManager manager) {

        if(this.currentState != null && this.isDead()) {
            //TODO: cancel state, and end
            this.currentState.onStateExit();
            this.currentState = null;
            return;
        } else if(this.isDead()) {
            return;
        } else if(this.currentState == null) {
            this.currentState = this.Idle;
            this.requestStateSwap();
        }

        //if we have no target or our target is dead: we want a new target and we cancel our current action
        if(this.currentTarget == null || this.currentTarget.isDead()) {
            this.currentTarget = this.manager.requestAttackTarget(this, this.currentTarget);
            if (this.currentState.cancelAction()) {
                StateDataContainer stateData = new StateDataContainer(this.currentTarget, null, .25f);
                this.Idle.onStateEnter(this.currentState, stateData);
                this.currentState = this.Idle;
                
            }
        }

        //try to cancel attack if the target is out of our range
        if(this.currentTarget != null && this.range < HexSquare.hexDist(this.currentTarget.getPosition(), this.getPosition())) {
            if (this.currentState.cancelAction()) {
                StateDataContainer stateData = new StateDataContainer(this.currentTarget, null, .25f);
                this.Idle.onStateEnter(this.currentState, stateData);
                this.currentState = this.Idle;
            }
        }

        currentState.update();
    }

    public void requestStateSwap() {
        StateDataContainer stateData = new StateDataContainer(this.currentTarget, null, -1f);
        if(this.currentTarget == null) {
            this.currentState.swapStates(this.Idle, stateData);
            this.currentState = this.Idle;
        } else if(this.canReachTarget()) {
            stateData.flt = this.attackSpeedCalculator.calculate(); //give attack speed into attack
            this.currentState.swapStates(this.Attack, stateData);
            this.currentState = this.Attack;
        } else {
            Unit newPossibleTarget = this.manager.requestAttackTarget(this, this.currentTarget);
            if(newPossibleTarget != null) {
                this.currentTarget = newPossibleTarget; //target reassigned :'(
            }

            stateData.sq = this.manager.requestRoute(this, this.currentTarget);
            this.currentState.swapStates(this.Walk, stateData);
            this.currentState = this.Walk;
        }
    }

    public void onCombatEnd() {
        if(!this.isDead()) {
            float rng = Random.Range(0f, 1f);
            if(rng < .05f) {
                this.triggerAnimation("victory_dance");
            } else if(rng < .4f) {
                this.triggerAnimation("victory_jump");
            } else {
                this.triggerAnimation("victory_clap");
            }
            this.hpBar.onDeath();
            this.hpBar = null;
        }
        
        this.healthCalculator.wipe();
        this.damageCalculator.wipe();
        this.attackSpeedCalculator.wipe();
    }

    public void placeUnitOnSquare(HexSquare square, bool move) {
        if(this.curPosition && this.curPosition.isSquare()) {
            this.curPosition.claimSquare(this, false);
        }
        this.curPosition = square;
        square.claimSquare(this, true);

        if(move) {
            this.transform.position = new Vector3(square.transform.position.x, this.transform.position.y, square.transform.position.z);
        }
    }

    public void placeUnitOnBench() {
        this.unclaimSquare();
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

    public void triggerAnimation(string trigger) {
        this.animator.SetTrigger(trigger);
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
        this.triggerAnimation("death");

        this.manager.onUnitDeath(this);
        this.curPosition.claimSquare(this, false);

        //careful that this isn't called more than once
        Invoke(nameof(die), 2.5f);
    }

    private void die() {
        this.hpBar.onDeath();
        this.hpBar = null;
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
                float oldMaxHp = this.healthCalculator.calculate();
                this.healthCalculator.addOperation(operation);
                float newMaxHp = this.healthCalculator.calculate();
                this.currentHealth = Mathf.Min(this.currentHealth + Mathf.Max(newMaxHp - oldMaxHp, 0), newMaxHp);

                this.hpBar.updateMaxHealth(newMaxHp); //TODO: other places this is updated?
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

