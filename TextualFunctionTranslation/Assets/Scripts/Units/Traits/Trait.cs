using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Trait : MonoBehaviour
{
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
    public virtual void runCypher(char[] curCypherText) {}

    public void registerUnit() {
        this.numberActive++;
    }

    //this should do whatever the trait does
    public virtual void onPrecombatStart(){
        this.numberActive = 0;
    }

    public virtual void onCombatStart(Unit traitHaver){}

    public virtual void onUnitAttack(Unit traitHaver, Unit target){} //target not needed but correct to have 

    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){}

    public virtual void onCombatSurvival(Unit traitHaver){}

    public virtual void onCombatEnd(Unit traitHaver){}


    //constructor work around cuz unity is a chud platform :(
    protected abstract string getTraitName();

    protected abstract int getMinimumForActive();

    public int getNumberActive() {
        return this.numberActive;
    }

/*

vegenery 2
clockmaker 4
royal guard 4
artisan 3
assasin 4
city watch 4
unnamed on-death trait 3

scout 4
druid 3
warden 4
marksman 4
outcast 3
tracker 4
plague doctor 2

01 - scout | royal guard
02 - scout | night watch
03 - scout | 
04 - scout |
05 - druid | vegenery
06 - druid | assasin
07 - druid | 
08 - warden |
09 - warden |
10 - warden |
11 - warden |
12 - marksman |
13 - marksman |
14 - marksman |
15 - marksman |
16 - outcast |
17 - outcast |
18 - outcast |
19 - tracker |
20 - tracker |
21 - tracker |
22 - tracker |
23 - plague doctor | assasin
24 - plague doctor | unnamed on-death trait
*/
}


