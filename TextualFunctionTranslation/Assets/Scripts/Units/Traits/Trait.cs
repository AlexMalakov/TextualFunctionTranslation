using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Trait : MonoBehaviour
{
    [SerializeField] private TraitDisplay traitDisplay;
    private TraitWidget tw;
    private string traitName;
    private int minimumForActivate;
    private int numberActive = 0;
    
    public bool attemptActivate(int numUnits) {
        if(numUnits < this.minimumForActivate) {
            return false;
        }

        return true;

        //TODO: ELSE DO STUFF !!!!!
    }

    public void Awake() { //I'm allowed to be an awake because I update trait widget by giving it myself, and static facts about myself
        this.tw = traitDisplay.createWidget();
        this.tw.init(this);
    }

    public TraitWidget getTraitWidget() {
        return this.tw;
    }

    protected List<HexSquare> getSquaresInRadius(HexSquare centerSq, int radius, HexBoard board) {
        List<HexSquare> squares = new List<HexSquare>();
        AxialCoord center = centerSq.getCoords();
        AxialCoord[] possibleNeighbors = {
            new AxialCoord(center.q + radius, center.r - radius, center.s),
            new AxialCoord(center.q + radius, center.r, center.s - radius),
            new AxialCoord(center.q - radius, center.r + radius, center.s),
            new AxialCoord(center.q, center.r + radius, center.s - radius),
            new AxialCoord(center.q - radius, center.r, center.s + radius),
            new AxialCoord(center.q, center.r - radius, center.s + radius),
        };

        foreach(AxialCoord c in possibleNeighbors) {
            HexSquare sq = board.getHexAt(c);
            if(sq.isSquare()) {
                squares.Add(sq);
            }
        }

        return squares;
    }

    //W polymorphism
    public virtual void runSquareReveal() {}
    public virtual void resetRevealedSquares() {}
    public virtual char[] runCypher(char[] curCypherText) {return curCypherText;}

    public void registerUnit(bool addingUnit) {
        if(addingUnit)
            this.numberActive++;
        else
            this.numberActive--;

        this.tw.update();
    }

    public void wipeCount() {
        this.numberActive = 0;
    }

    //these should do whatever the trait does
    public virtual void onPrecombatStart() {}

    public virtual void onCombatStart(Unit traitHaver){}

    public virtual void onUnitAttack(Unit traitHaver, Unit target){} //target not needed but correct to have 

    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){}

    public virtual void onArbitraryDeath(Unit traitHaver, Unit target){}

    public virtual void onCombatSurvival(Unit traitHaver){}

    public virtual void onUnitDeath(Unit traitHaver, Unit killer) {}

    //THESE LAST 2 are kinda ass they only exist for one trait
    public virtual void informAllOffDeath(Unit died){}

    public virtual void preCombatFinal(Unit traitHaver) {}

    public virtual void onCombatEnd(Unit traitHaver){}


    //constructor work around cuz unity is a chud platform :(
    public abstract string getTraitName();

    protected abstract int getMinimumForActive();

    public abstract List<int> getTraitTiering();

    public int getNumberActive() {
        return this.numberActive;
    }
/*

vegenery 2
clockmaker 4
royal guard 4
artisan 3
assasin 4
night watch 4
martyr 3

scout 4
druid 3
warden 4
marksman 4
outcast 3
tracker 4
plague doctor 2

01 - scout | royal guard
02 - scout | night watch
03 - scout | martyr
04 - scout | assasin
05 - druid | vegenery
06 - druid | assasin
07 - druid | artisan
08 - warden | martyr
09 - warden | royal guard
10 - warden | night watch
11 - warden | clockmaker
12 - marksman | royal guard
13 - marksman | artisan
14 - marksman | clockmaker
15 - marksman | vegenery
16 - outcast | night watch
17 - outcast | martyr
18 - outcast | artisan
19 - tracker | royal guard
20 - tracker | assasin
21 - tracker | night watch
22 - tracker | clockmaker
23 - plague doctor | assasin
24 - plague doctor | clockmaker
*/
}


