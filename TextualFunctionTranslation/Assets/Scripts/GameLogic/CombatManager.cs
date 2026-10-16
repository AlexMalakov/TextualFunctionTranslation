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
    [SerializeField] private HeroCheck heroCheck;

    [SerializeField] private AssasinsGuildTrait playerAss;
    [SerializeField] private AssasinsGuildTrait enemyAss;

    private List<Unit> units;
    private Dictionary<Unit, int> iframes = new Dictionary<Unit, int>();
    private bool combatEnded = false;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;

    public void initPlayers(PlayerUnitLayout enemyLayout, string allLeters, int unitCap) {
        this.board.assignLetters(allLeters);

        this.player.setUnitCap(unitCap);
        this.player.init();
        this.enemy.assignLayout(enemyLayout);
        this.enemy.init();
        this.enemy.claimUnitsFromLayout();
        
        Unit mimicPresent = null;
        foreach(Unit u in this.enemy.getActiveUnits()) {
            if(u is Mimic) {
                mimicPresent = u;
                break;
            }
        }

        foreach(Trait t in this.player.getAllTraits()) {
            t.wipeCount();
        }

        foreach(Trait t in this.enemy.getAllTraits()) {
            t.wipeCount();
            if(mimicPresent != null) {
                t.registerUnit(true, mimicPresent);
            }
        }

        foreach(Unit u in this.player.getActiveUnits()) {
            u.getRoleTrait().registerUnit(true, u);
            u.getClassTrait().registerUnit(true, u);
        }

        foreach(Unit u in this.enemy.getActiveUnits()) {
            u.getRoleTrait().registerUnit(true, u);
            u.getClassTrait().registerUnit(true, u);
        }

        this.traitDisplay.onUnitLayoutUpdate();
    }

    public void placeUnitsFromLayouts() {
        this.player.getUnitLayout().placeUnits();
        this.enemy.getUnitLayout().placeUnits();
    }

    public void initializeCombat() {
        this.units = new List<Unit>();
        this.iframes = new Dictionary<Unit, int>();
        this.units.AddRange(this.player.getActiveUnits());
        this.units.AddRange(this.enemy.getActiveUnits());
        this.livingPlayerUnits = this.player.getActiveUnits().Count;
        this.livingEnemyUnits = this.enemy.getActiveUnits().Count;
        this.units = this.units.OrderBy(u => u.getDecisionPriority()).ThenByDescending(u => u.getMergeLevel()).ToList();//TODO CHECK: is this deterministic?

        this.combatEnded = false;

        this.placeUnitsFromLayouts();

        this.combatLog.initLog(this.units);

        this.traitPreCombat();

        this.heroCheck.precombatCheck();
    }

    public void requestHealthBar(Unit u) {
        u.assignHpBar(Instantiate(this.healthbarPrefab).GetComponent<HealthBarManager>(), this.hpCanvas);
    }

    public IEnumerator precombatAnims() {
        foreach(Unit u in this.units) {
            if(u is DemonKing) {
                u.triggerAnimation("king_precombat");
            } else if(!((this.playerAss.attemptActivate(this.playerAss.getNumberActive()) && this.isPlayerUnit(u)) 
                        || (this.enemyAss.attemptActivate(this.enemyAss.getNumberActive()) && !this.isPlayerUnit(u)))) {
                u.triggerAnimation("precombat");
            }
        }

        yield return new WaitForSeconds(precombatAnimTime);
    }

    //check each unit every frame
    public void runAttackFrame() {
        if(this.livingPlayerUnits <= 0 || this.livingEnemyUnits <= 0) {
            this.onCombatOver();
            return;
        }

        foreach(Unit u in this.units) {
            if(!u.isDead()) {
                u.triggerUpdate(this);
            }
        }

        List<Unit> iUnits = this.iframes.Keys.ToList();
        for(int i = this.iframes.Count - 1; i >= 0; i--) {
            this.iframes[iUnits[i]] = this.iframes[iUnits[i]] - 1;
            if(this.iframes[iUnits[i]] == 0) {
                this.iframes.Remove(iUnits[i]);
                iUnits.RemoveAt(i);
            }
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

    public void addInvulUnit(Unit registerMe, float invulDurationInSec) {
        this.iframes.Add(registerMe, CombatScheduler.timeToTicks(invulDurationInSec));
    }

    public Unit requestAttackTarget(Unit attacker, Unit previousTarget) {
        Dictionary<HexSquare, int> dists = bfsTheBoard(attacker.getPosition());

        int bestTaunt = int.MinValue;
        int closestTarget = int.MaxValue;
        Unit target = null;
        int prevDist = int.MaxValue;

        if(previousTarget != null && !previousTarget.isDead()) {
            target = previousTarget;
            bestTaunt = previousTarget.getTaunt();
            closestTarget = reachCost(attacker, previousTarget, dists);
            prevDist = closestTarget;
        }

        foreach(Unit u in this.units) {
            if(u.isDead() || sameOwner(attacker, u) || this.iframes.ContainsKey(u))
                continue;
            
            int d = reachCost(attacker, u, dists);
            bool better = false;
            if(target == null) {
                better = true;
            } else if(target == previousTarget) {
                better = (d <= closestTarget - 1 || (d < closestTarget && u.getTaunt() > bestTaunt));
            } else {
                better = (d < closestTarget || (d == closestTarget && u.getTaunt() > bestTaunt));
            }
            
            if(better) {
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
            if(this.isPlayerUnit(u) && !u.isDead() && !this.iframes.ContainsKey(u)) {
                playerPool.Add(u);
            }
        }

        if(playerPool.Count == 0) {
            return null;
        }
        return playerPool[rand.Next(0, playerPool.Count)];
    }

    private Dictionary<HexSquare, int> bfsTheBoard(HexSquare start) {
        Dictionary<HexSquare, int> dist = new Dictionary<HexSquare, int>(){{start, 0}};
        Queue<HexSquare> q = new Queue<HexSquare>();
        q.Enqueue(start);
        while(q.Count > 0) {
            HexSquare next = q.Dequeue();
            foreach(AxialCoord c in next.getNeighborCoords()) {
                HexSquare n = this.board.getHexAt(c);
                if(n == null || !n.isSquare() || n.getUnit() != null || dist.ContainsKey(n)) {
                    continue;
                }
                dist[n] = dist[next] + 1;
                q.Enqueue(n);
            }
        }

        return dist;
    }

    private int reachCost(Unit attacker, Unit target, Dictionary<HexSquare, int> dist) {
        if(HexSquare.hexDist(attacker.getPosition(), target.getPosition()) <= attacker.getRange()) {
            return 0;
        }
        int best = int.MaxValue;
        foreach(HexSquare s in dist.Keys) {
            if(dist[s] < best && HexSquare.hexDist(s, target.getPosition()) <= attacker.getRange()) {
                best = dist[s];
            }
        }
        return best;
    }

    public HexSquare requestRoute(Unit attacker, Unit target) {
        return this.board.pathfindToSquare(attacker.getPosition(), target.getPosition());
    }

    public void heroDeathCheck(Unit u) {
        this.heroCheck.onDeathCheck(u);
    }

    public void onUnitDeath(Unit u) {
        if(u is Randomizer) {
            this.gameManager.onRandomizerDeath();
        }

        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            callTraitMethod(t, t.informAllOffDeath, u);
        }

        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            callTraitMethod(t, t.informAllOffDeath, u);
        }

        if(player.ownsUnit(u)) {
            this.livingPlayerUnits--;
            if(this.livingPlayerUnits <= 0) {
                this.onCombatOver();
            }
        } else if(enemy.ownsUnit(u)) {
            this.livingEnemyUnits--;
            if(this.livingEnemyUnits <= 0) {
                this.onCombatOver();
            }
        } else {
            throw new Exception("im dead but not in any list something is wrong");
        }
    }

    public void onUnitReposition(Unit u) {
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            callTraitMethod(t, t.informAllOfReposition, u);
        }

        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            callTraitMethod(t, t.informAllOfReposition, u);
        }
    }

    public void onCombatOver() {
        if(this.combatEnded) {
            return;
        }
        this.combatEnded = true;
        foreach(Unit u in this.units) {
            if(!u.isDead()) {
                callTraitMethod(u.getRoleTrait(), u.getRoleTrait().onCombatSurvival, u);
                callTraitMethod(u.getClassTrait(), u.getClassTrait().onCombatSurvival, u);
            }
        }

        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            callTraitMethod(t, t.onCombatEnd);
        }

        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            callTraitMethod(t, t.onCombatEnd);
        }

        foreach(Unit u in this.units) {
            u.onCombatEnd();
        }

        if(this.livingPlayerUnits <= 0) {
            this.gameManager.restartLevel();
        } else {
            this.gameManager.continueToNextLevel();
        }
    }

    public void wipeEnemyLayout() {
        this.enemy.wipeLayoutUnits();
    }

    public IEnumerator runSquaresReveal() {
        yield return this.traitDisplay.playTraitSquareReveal();
        foreach(Unit u in this.units) {
            if(u is DemonKing) {
                u.getPosition().highlightSquare(new Color(0, 0, 0), 1f);   
                yield return new WaitForSeconds(1f);
                yield return this.traitDisplay.playTraitSquareReveal();
            }
        }
        
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
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        char[] alphabetArray = alphabet.ToCharArray();
        Debug.Log("encryption test: CAPTURING SUBSTITUTION USING ALPHABET: " + alphabet);

        char[] cypherArray = plaintext.ToCharArray();

        //TO CHECK: make sure this is same order as animations are
        foreach(Trait t in this.player.getUnitLayout().getActiveTraits().OrderByDescending(t => t.getTraitName())) {
            cypherArray = t.runCypher(cypherArray);
            if(t is CypherTrait) {
                Debug.Log("encryption test: ENCRYPTED WITH TRAIT " + t.gameObject.name);
                alphabetArray = t.runCypher(alphabetArray);
                Debug.Log("encryption test: ENCRYPTED WITH TRAIT " + new string(alphabetArray));
            }
            
        }
        
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits().OrderBy(t => t.getTraitName())) {
            cypherArray = t.runCypher(cypherArray);
            if(t is CypherTrait) {
                Debug.Log("encryption test: ENCRYPTED WITH TRAIT " + t.gameObject.name);
                alphabetArray = t.runCypher(alphabetArray);
                Debug.Log("encryption test: ENCRYPTED WITH TRAIT " + new string(alphabetArray));
            }
        }

        return new string(cypherArray);
    }
    
    public IEnumerator runCypherAnimation() {
        yield return this.traitDisplay.playCypherTraitAnim(false);
        foreach(Unit u in this.units) {
            if(u is DemonKing) {
                u.getPosition().highlightSquare(new Color(0, 0, 0), 1f);   
                yield return new WaitForSeconds(1f);
                yield return this.traitDisplay.playCypherTraitAnim(true);
            }
        }
    }

    private void traitPreCombat() {
        //ok so briliant idea: since cypher is only called on active traits, i can just have these methods get pinged constantly for inactive traits
        //I just need to make sure all traits get their shit wiped at the end

        //This idea is not briliant these like provide buffs and shit bro like what are we doing...

        foreach(Trait t in this.player.getUnitLayout().getActiveTraits()) {
            t.resetRevealedSquares();
            t.resetCypher();
            callTraitMethod(t, t.onPrecombatStart);
        }
        foreach(Trait t in this.enemy.getUnitLayout().getActiveTraits()) {
            t.resetRevealedSquares();
            t.resetCypher();
            callTraitMethod(t, t.onPrecombatStart);
        }

        foreach(Unit u in this.units) {
            callTraitMethod(u.getRoleTrait(), u.getRoleTrait().onCombatStart, u);
            callTraitMethod(u.getClassTrait(), u.getClassTrait().onCombatStart, u);
        }

        foreach(Unit u in this.units) {
            callTraitMethod(u.getRoleTrait(), u.getRoleTrait().preCombatFinal, u);
            callTraitMethod(u.getClassTrait(), u.getClassTrait().preCombatFinal, u);
        }
    }

    public void resetToPostCombat() {
        Dictionary<Unit, HexSquare> startingPos = this.combatLog.getInitialPositions();

        foreach(Unit u in this.units) {
            u.unclaimSquare();
            u.bringBackHPBar();
        }

        foreach(Unit u in this.units) { //TODO: make sure no new units end up in combat somehow -- lock playerunit layout when not in prep phase i think
            if(!this.player.ownsUnit(u) && !this.enemy.ownsUnit(u)) {
                continue;
            }
            
            if(startingPos.ContainsKey(u)) {
                u.placeUnitOnSquare(startingPos[u], true);
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
                u.hideHPBar();
                u.gameObject.SetActive(false);
            } else {
                Debug.Log("UNKNOWN KEY FROM UNIT " + u.gameObject.name);
            }
        }
    }

    public static void callTraitMethod(Trait t, Action<Unit, Unit> traitFunc, Unit arg1, Unit arg2) {
        if(t.attemptActivate(t.getNumberActive())) {
            traitFunc(arg1, arg2);
        }
    }
    public static void callTraitMethod(Trait t, Action<Unit> traitFunc, Unit arg1) {
        if(t.attemptActivate(t.getNumberActive())) {
            traitFunc(arg1);
        }
    }
    public static void callTraitMethod(Trait t, Action traitFunc) {
        if(t.attemptActivate(t.getNumberActive())) {
            traitFunc();
        }
    }
}