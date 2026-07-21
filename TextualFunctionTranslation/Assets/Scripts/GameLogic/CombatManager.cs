using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

    private List<Unit> units;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;

    public void initPlayers(PlayerUnitLayout enemyLayout, string allLeters, int unitCap) {
        this.player.setUnitCap(unitCap);
        this.player.init();
        this.enemy.assignLayout(enemyLayout);
        this.enemy.init();

        foreach(Unit u in this.player.getActiveUnits()) {
            u.getRoleTrait().wipeCount();
            u.getClassTrait().wipeCount();
        }

        foreach(Unit u in this.enemy.getActiveUnits()) {
            u.getRoleTrait().wipeCount();
            u.getClassTrait().wipeCount();
        }

        foreach(Unit u in this.player.getActiveUnits()) {
            u.getRoleTrait().registerUnit(true);
            u.getClassTrait().registerUnit(true);
        }

        foreach(Unit u in this.enemy.getActiveUnits()) {
            u.getRoleTrait().registerUnit(true);
            u.getClassTrait().registerUnit(true);
        }

        this.traitDisplay.onUnitLayoutUpdate();
        this.board.assignLetters(allLeters);
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
        this.units.OrderBy(u => u.getDecisionPriority());//TO CHECK: is this deterministic?
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

                this.combatLog.logEvent(new CombatEvent(u, u, CombatEventType.Survival));
            }

            u.getRoleTrait().onCombatEnd(u);
            u.getClassTrait().onCombatEnd(u);

        }

        foreach(Unit u in this.units) {
            u.onCombatEnd();
        }

        this.gameManager.onCombatEnd();
    }

    public IEnumerator runSquaresReveal() {
        yield return this.traitDisplay.playTraitSquareReveal();
        
    }

    public string encrypt(string plaintext) {
        char[] cypherArray = plaintext.ToCharArray();

        //TO CHECK: make sure this is same order as animations are
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            t.runCypher(cypherArray);
        }
        
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderByDescending(t => t.getTraitName())) {
            t.runCypher(cypherArray);
        }
        
        return new string(cypherArray);
    }
    
    public IEnumerator runCypherAnimation() {
        yield return this.traitDisplay.playCypherTraitAnim();
    }

    private void traitPreCombat() {
        //ok so briliant idea: since cypher is only called on active traits, i can just have these methods get pinged constantly for inactive traits
        //I just need to make sure all traits get their shit wiped at the end

        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            t.resetRevealedSquares();
            t.onPrecombatStart();
        }
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            t.resetRevealedSquares();
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