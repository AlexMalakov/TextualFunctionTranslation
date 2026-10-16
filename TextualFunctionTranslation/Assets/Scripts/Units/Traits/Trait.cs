using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Trait : MonoBehaviour
{
    [SerializeField] private TraitDisplay traitDisplay;
    [SerializeField] private Sprite traitIcon;
    [SerializeField] private Sprite effectImg;
    [SerializeField] private TraitCodexInfo tci;
    [SerializeField] private GameObject traitExplainerMenu;
    [SerializeField] protected CombatLog log;
    [SerializeField] protected bool playersTrait;
    private TraitWidget tw;
    private string traitName;
    private HashSet<Unit> activeUnits = new HashSet<Unit>();
    
    public bool attemptActivate(int numUnits) {
        return numUnits >= this.getMinimumForActive();
    }

    public void Awake() { //I'm allowed to be an awake because I update trait widget by giving it myself, and static facts about myself
        this.tw = traitDisplay.createWidget(playersTrait, this is CypherTrait);
        this.tw.init(this, this.tci, this.traitExplainerMenu);
    }

    public TraitWidget getTraitWidget() {
        return this.tw;
    }

    //dude this was absurdly bugged so i fully rewrote it lmaooo
    protected List<HexSquare> getSquaresInRadius(HexSquare centerSq, int radius, HexBoard board) {
        List<HexSquare> squares = new List<HexSquare>();
        AxialCoord center = centerSq.getCoords();
        AxialCoord[] directions = {
            new AxialCoord(1, -1, 0), 
            new AxialCoord(1, 0, -1),
            new AxialCoord(0, 1, -1),
            new AxialCoord(-1, 1, 0),
            new AxialCoord(-1, 0, 1),
            new AxialCoord(0, -1, 1),
        };

        AxialCoord walk = new AxialCoord(center.q - radius, center.r, center.s + radius);

        foreach(AxialCoord walkDir in directions) {
            for(int i = 0; i < radius; i++) {
                HexSquare sq = board.getHexAt(walk);
                if(sq.isSquare()) {
                    squares.Add(sq);
                }
                walk = new AxialCoord(walk.q + walkDir.q, walk.r + walkDir.r, walk.s + walkDir.s);
            }
        }

        return squares;
    }

    //W polymorphism
    public virtual void runSquareReveal() {}
    public virtual void resetRevealedSquares() {}
    public virtual void resetCypher() {}
    public virtual char[] runCypher(char[] curCypherText) {return curCypherText;}

    public virtual void registerUnit(bool addingUnit, Unit u) {
        if(addingUnit)
            this.activeUnits.Add(u);
        else
            this.activeUnits.Remove(u);

        this.tw.update();
    }

    public void wipeCount() {
        this.activeUnits = new HashSet<Unit>();
    }

    //these should do whatever the trait does
    public virtual void onPrecombatStart() {}

    public virtual void onCombatStart(Unit traitHaver){}

    public virtual void onUnitAttack(Unit traitHaver, Unit target){} //target not needed but correct to have 

    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){}

    public virtual void onCombatSurvival(Unit traitHaver){}

    public virtual void onUnitDeath(Unit traitHaver, Unit killer) {}

    //THESE LAST 2 are kinda ass they only exist for one trait
    public virtual void informAllOffDeath(Unit died){}
    public virtual void informAllOfReposition(Unit mover){}

    public virtual void preCombatFinal(Unit traitHaver) {}

    public virtual void onCombatEnd(){}

    public virtual void onReposition(Unit traitHaver) {}

    //constructor work around cuz unity is a chud platform :(
    public abstract string getTraitName();
    public abstract string getTraitNarrative();
    public abstract string getTraitEffect();

    public abstract int getMinimumForActive();

    public abstract List<int> getTraitTiering();

    public int getNumberActive() {
        return this.activeUnits.Select(u => u.getName()).Distinct().ToList().Count;
    }
    
    public Sprite getTraitIcon() {
        return this.traitIcon;
    }

    public Sprite getTraitEffectImg() {
        return this.effectImg;
    }
/*

vegenery 2
clockmaker 4
royal guard 4
artisan 3
assassin 4
night watch 4
goblin 3

scout 4
druid 3
warden 4
marksman 4
outcast 3
tracker 4
plague doctor 2

01 - scout | royal guard
02 - scout | night watch
03 - scout | goblin
04 - scout | assassin
05 - druid | troll
06 - druid | assassin
07 - druid | artisan
08 - warden | goblin
09 - warden | royal guard
10 - warden | night watch
11 - warden | clockmaker
12 - marksman | royal guard
13 - marksman | artisan
14 - marksman | clockmaker
15 - marksman | vegenery
16 - outcast | night watch
17 - outcast | goblin
18 - outcast | artisan
19 - tracker | royal guard
20 - tracker | assassin
21 - tracker | night watch
22 - tracker | clockmaker
23 - plague doctor | assassin
24 - plague doctor | clockmaker
*/
}


