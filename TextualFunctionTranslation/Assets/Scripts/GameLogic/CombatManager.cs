using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//idea: each pla
public class CombatManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private HexBoard board;
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Transform hpCanvas;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CombatLog combatLog;
    [SerializeField] private TraitDisplay traitDisplay;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;
    private List<Unit> units;

    public void initPlayers(PlayerUnitLayout enemyLayout) {
        foreach(Unit u in this.units) {
            u.getRoleTrait().wipeCount();
            u.getClassTrait().wipeCount();
        }

        this.player.init();
        this.enemy.assignLayout(enemyLayout);
        this.traitDisplay.onUnitLayoutUpdate();
        this.enemy.init();
    }

    public void placeUnitsFromLayouts() {
        this.player.getUnitLayout().placeUnits(this.healthbarPrefab, this.hpCanvas);
        this.enemy.getUnitLayout().placeUnits(this.healthbarPrefab, this.hpCanvas);
    }

    public void initializeCombat() {
        this.units = new List<Unit>();
        this.units.AddRange(this.player.getActiveUnits());
        this.units.AddRange(this.enemy.getActiveUnits());
        livingPlayerUnits = this.player.getActiveUnits().Count;
        livingEnemyUnits = this.enemy.getActiveUnits().Count;
        this.units.Sort((a,b) => a.getDecisionPriority().CompareTo(b.getDecisionPriority())); //TODO: swap with inumerable.OrderBy to make this stable so its deterministic
        //TODO: knob to tune: tiebreak behavior. Does player get to win ties?6

        this.placeUnitsFromLayouts();

        this.combatLog.initLog(this.units);

        this.traitPreCombat();
    }

    //check each unit every frame
    public void runAttackFrame() {
        foreach(Unit u in this.units) {
            u.checkUnitHasTask(this);
        }
    }

    public HexBoard getBoard() {
        return this.board;
    }

    public bool sameOwner(Unit unit1, Unit unit2) {
        return (this.player.ownsUnit(unit1) && this.player.ownsUnit(unit2))
            || (this.enemy.ownsUnit(unit1) && this.enemy.ownsUnit(unit2));
    }

    //this might already exist if so oops
    public bool isPlayerUnit(Unit unit) {
        return this.player.ownsUnit(unit);
    }

    public Unit requestAttackTarget(Unit attacker, Unit previousTarget) {
        if(previousTarget.isDead()) {
            this.combatLog.logEvent(new CombatEvent(attacker, previousTarget, CombatEventType.attack));
            attacker.getRoleTrait().onEnemyDefeat(attacker, previousTarget);
            attacker.getClassTrait().onEnemyDefeat(attacker, previousTarget);
            attacker.getRoleTrait().onUnitDeath(previousTarget, attacker);
            attacker.getClassTrait().onUnitDeath(previousTarget, attacker);
        }

        Func<HexSquare, bool> getAttackableSquares = (HexSquare s) => {
            return s.getUnit() != null
                //TODO: && can reach that unit?
                && !sameOwner(attacker, s.getUnit());
        };

        List<HexSquare> possibleTargets = this.board.filterOperationOnSquares(getAttackableSquares);

        int bestTaunt = int.MinValue;
        int closestTarget = int.MaxValue;
        Unit target = null;
        foreach(HexSquare s in possibleTargets) {
            if(s.getUnit() == previousTarget) {
                return previousTarget;
            }

            if(HexSquare.hexDist(attacker.getPosition(), s) < closestTarget) {
                target = s.getUnit();
                bestTaunt = target.getTaunt();
                closestTarget = HexSquare.hexDist(attacker.getPosition(), s);
                continue;
            }
            if(s.getUnit().getTaunt() > bestTaunt) {
                target = s.getUnit();
                bestTaunt = target.getTaunt();
            }
        }

        //TODO: handle might return null?
        return target;
    }

    public HexSquare requestRoute(Unit attacker, Unit target) {
        return this.board.pathfindToSquare(attacker.getPosition(), target.getPosition());
    }

    public void onUnitDeath(Unit u) {
        if(player.ownsUnit(u)) {
            this.livingPlayerUnits--;
            if(livingPlayerUnits <= 0) {
                this.onCombatOver();
            }
        } else if(enemy.ownsUnit(u)) {
            this.livingEnemyUnits--;
            if(livingEnemyUnits <= 0) {
                this.onCombatOver();
            }
        } else {
            throw new Exception("im dead but not in any list something is wrong");
        }
        units.Remove(u);
    }

    public void onCombatOver() {
        foreach(Unit u in this.units) {
            if(!u.isDead()) {
                u.getRoleTrait().onCombatSurvival(u);
                u.getClassTrait().onCombatSurvival(u);
            }

            u.getRoleTrait().onCombatEnd(u);
            u.getClassTrait().onCombatEnd(u);

        }

        foreach(Unit u in this.units) {
            u.onCombatEnd();
        }

        this.gameManager.onCombatEnd();
    }

    public void runSquaresReveal() {

        //TODO: sort alphabetically
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            t.runSquareReveal();
        }
        
        //TODO: sort reverse alphabetically
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            t.runSquareReveal();
        }
        
    }

    public string encrypt(string plaintext) {
        char[] cypherArray = plaintext.ToCharArray();

        //TODO: sort alphabetically
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            t.runCypher(cypherArray);
        }
        
        //TODO: sort reverse alphabetically
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            t.runCypher(cypherArray);
        }
        
        return new string(cypherArray);
    }
    
    public void runCypherAnimation() {
        //TODO: this should start a coroutine I think?
    }

    private void traitPreCombat() {
        //ok so briliant idea: since cypher is only called on active traits, i can just have these methods get pinged constantly for inactive traits
        //I just need to make sure all traits get their shit wiped at the end

        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            t.onPrecombatStart();
        }
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            t.onPrecombatStart();
        }

        foreach(Unit u in this.units) {
            u.getRoleTrait().onCombatStart(u);
            u.getClassTrait().onCombatStart(u);
        }

        foreach(Unit u in this.units) {
            u.getRoleTrait().preCombatFinal(u);
            u.getClassTrait().preCombatFinal(u);
        }
    }
}