using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//idea: each pla
public class CombatManager : MonoBehaviour
{
    [SerializeField] private HumanPlayer player;
    [SerializeField] private Player enemy;
    [SerializeField] private HexBoard board;
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Transform hpCanvas;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CombatLog combatLog;
    [SerializeField] private TraitDisplay traitDisplay;
    [SerializeField] private float precombatAnimTime = 2f;

    private List<Unit> units;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;

    public void initPlayers(PlayerUnitLayout enemyLayout, string allLeters, int unitCap) {
        this.player.setUnitCap(unitCap);
        this.player.init();
        this.enemy.assignLayout(enemyLayout);
        this.enemy.init();
        this.enemy.claimUnitsFromLayout();
        
        bool mimicPresent = false;
        foreach(Unit u in this.enemy.getActiveUnits()) {
            mimicPresent = mimicPresent || (u is Mimic);
        }

        foreach(Trait t in this.player.getAllTraits()) {
            t.wipeCount();
        }

        foreach(Trait t in this.enemy.getAllTraits()) {
            t.wipeCount();
            if(mimicPresent) {
                t.registerUnit(true);
            }
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
        this.units.OrderBy(u => u.getDecisionPriority());//TODO CHECK: is this deterministic?

        this.placeUnitsFromLayouts();

        this.combatLog.initLog(this.units);

        this.traitPreCombat();
    }

    public IEnumerator precombatAnims() {
        foreach(Unit u in this.units) {
            u.triggerAnimation("draw_weapon"); //TODO: only can transition from idle, not from assasin jump >:( 
        }

        yield return new WaitForSeconds(precombatAnimTime);
    }

    //check each unit every frame
    public void runAttackFrame() {
        foreach(Unit u in this.units) {
            if(!u.isDead())
                u.triggerUpdate(this);
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
        int bestTaunt = int.MinValue;
        int closestTarget = int.MaxValue;
        Unit target = null;
        int prevDist = int.MaxValue;

        if(previousTarget != null && !previousTarget.isDead()) {
            target = previousTarget;
            bestTaunt = previousTarget.getTaunt();
            closestTarget = HexSquare.hexDist(previousTarget.getPosition(), attacker.getPosition());
            prevDist = closestTarget;
        }

        foreach(Unit u in this.units) {
            if(u.isDead())
                continue;
            int d = HexSquare.hexDist(u.getPosition(), attacker.getPosition());
            if(!sameOwner(attacker, u) && (closestTarget > d || (closestTarget == d && bestTaunt < u.getTaunt() && d < prevDist))) {
                target = u;
                bestTaunt = u.getTaunt();
                closestTarget = d;
            }
        }

        return target;
    }

    public Unit getRandomAttackTarget(System.Random rand) {
        List<Unit> playerPool = new List<Unit>();
        foreach(Unit u in this.units) {
            if(this.isPlayerUnit(u) && !u.isDead()) {
                playerPool.Add(u);
            }
        }

        return playerPool[rand.Next(0, playerPool.Count)];
    }

    public HexSquare requestRoute(Unit attacker, Unit target) {
        return this.board.pathfindToSquare(attacker.getPosition(), target.getPosition());
    }

    public void onUnitDeath(Unit u) {
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            t.informAllOffDeath(u);
        }

        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            t.informAllOffDeath(u);
        }

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
    }

    public void onCombatOver() {
        bool randomizerSurvived = false;
        foreach(Unit u in this.units) {
            if(!u.isDead()) {
                u.getRoleTrait().onCombatSurvival(u);
                u.getClassTrait().onCombatSurvival(u);

                this.combatLog.logEvent(new CombatEvent(u, u, CombatEventType.Survival));

                if(u is Randomizer) {
                    randomizerSurvived = true;
                }
            }

            u.getRoleTrait().onCombatEnd(u);
            u.getClassTrait().onCombatEnd(u);

        }

        foreach(Unit u in this.units) {
            u.onCombatEnd();
        }

        if(this.livingPlayerUnits <= 0) {
            this.gameManager.restartLevel(randomizerSurvived);
        } else {
            this.gameManager.continueToNextLevel(randomizerSurvived);
        }
    }

    public IEnumerator runSquaresReveal() {
        yield return this.traitDisplay.playTraitSquareReveal();
        
    }

    public string encrypt(string plaintext) {
        string ans = this.runEncrypt(plaintext);
        
        foreach(Unit u in this.units) {
            if(u is DemonKing) {
                ans = this.runEncrypt(ans);
                break;
            }
        }
        
        return ans;
    }

    private string runEncrypt(string plaintext) {
        char[] cypherArray = plaintext.ToCharArray();

        //TO CHECK: make sure this is same order as animations are
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            cypherArray = t.runCypher(cypherArray);
        }
        
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderByDescending(t => t.getTraitName())) {
            cypherArray = t.runCypher(cypherArray);
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

    public void resetToPostCombat() {
        Dictionary<Unit, HexSquare> startingPos = this.combatLog.getInitialPositions();

        foreach(Unit u in this.units) { //TODO: make sure no new units end up in combat somehow -- lock playerunit layout when not in prep phase i think
            if(startingPos.ContainsKey(u)) {
                u.placeUnitOnSquare(startingPos[u], true); //TODO: reset direction they're facing
                u.rotateTo((this.isPlayerUnit(u) ? this.player : this.enemy).getUnitRotation(false));
                u.gameObject.SetActive(true);
            } else {
                Debug.Log("UNKNOWN KEY FROM UNIT " + u.gameObject.name);
            }
        }
    }

    public void hideAllUnits() {
        Dictionary<Unit, HexSquare> startingPos = this.combatLog.getInitialPositions();
        foreach(Unit u in this.units) { //TODO: make sure no new units end up in combat somehow -- lock playerunit layout when not in prep phase i think
            if(startingPos.ContainsKey(u)) {
                u.gameObject.SetActive(false);
            } else {
                Debug.Log("UNKNOWN KEY FROM UNIT " + u.gameObject.name);
            }
        }
    }
}