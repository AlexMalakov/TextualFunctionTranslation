using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//struct linking a unit to a position
[Serializable]
public struct USPair
{
    public Unit unit;
    public OffsetCoord coord;
}

//stores the units a player has on the board: their position and the number of each trait that is present
public class PlayerUnitLayout : MonoBehaviour
{
    private Dictionary<Unit, HexSquare> layout;
    private Dictionary<Trait, int> traitCount = new Dictionary<Trait, int>();

    [SerializeField] private List<USPair> startingLayout;
    [SerializeField] private Player player;
    [SerializeField] private HexBoard board;
     
    //initializes the layout from starting layout, and sets up initial traits
    public void initLayout() {
        this.layout = new Dictionary<Unit, HexSquare>();
        foreach(USPair u in this.startingLayout) {
            this.layout.Add(u.unit, this.board.getHexAt(u.coord));
            this.addTrait(u.unit);
        }
    }

    //given a unit, add the role trait and class trait to the traits dic
    private void addTrait(Unit u) {
        Trait rt = u.getRoleTrait();
        if(this.traitCount.ContainsKey(rt)) {
            this.traitCount[rt] += 1;
        } else {
            this.traitCount.Add(rt, 1);
        }

        Trait ct = u.getClassTrait();
        if(this.traitCount.ContainsKey(ct)) {
            this.traitCount[ct] += 1;
        } else {
            this.traitCount.Add(ct, 1);
        }
    }

    //given a unit, remove its role and class traits from the dict
    private void removeTrait(Unit u) {
        Trait rt = u.getRoleTrait();
        this.traitCount[rt] -= 1;
        if(this.traitCount[rt] == 0) {
            this.traitCount.Remove(rt);
        }

        Trait ct = u.getClassTrait();
        this.traitCount[ct] -= 1;
        if(this.traitCount[ct] == 0) {
            this.traitCount.Remove(ct);
        }
    }
    
    //return false if fails
    //this is ugly as shit but womp womp
    public bool attemptUpdateLayout(Unit u, HexSquare square) { //moves unit u inot square s
        //can't move unit you don't own
        if(!this.board.getPlayerSqures().Contains(square) || !this.player.ownsUnit(u)) {
            return false;
        }
        //if unit already in the layout, it is being moved around
        if(this.layout.ContainsKey(u)) {
            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                u.unclaimSquare();
                this.layout[swapper] = this.layout[u];
                swapper.placeUnitOnSquare(this.layout[u]);
                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, this.layout[u].transform, false));
            } 
            this.layout[u] = square;
        //unit being added into layout
        } else {
            if(this.player.getMaxCombatUnits() <= this.layout.Count && (square.getUnit() == null || !this.player.getBench().canAddUnitToBench())) {
                return false;
            }

            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout.Remove(swapper);
                this.removeTrait(swapper);
                swapper.unclaimSquare();
                BenchSlot dest = this.player.getBench().addUnitToBench(swapper);

                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, dest.transform, false));
            }
            this.layout.Add(u, square);
            this.addTrait(u);
        }
        u.placeUnitOnSquare(square);
        StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, square.transform, true));

        return true;
    }

    public void addUnitFromShop(Unit u) {
        foreach(HexSquare s in this.board.getPlayerSqures()) {
            if(s.getUnit() == null) {
                this.layout.Add(u, s);
                this.addTrait(u);
                u.placeUnitOnSquare(s);
                return;
            }
        }
        throw new Exception("cannot place unit in layout despite player unit cap allowing it!!!");
    }

    public void removeUnitFromLayout(Unit u) {
        this.layout.Remove(u);
        this.removeTrait(u);
    }

    public List<Unit> getActiveUnits() {
        return new List<Unit>(this.layout.Keys);
    }

    public void placeUnits(GameObject hpBarPrefab, Transform hpCanvas) {
        foreach (KeyValuePair<Unit, HexSquare> kvp in this.layout) {
            Unit u = kvp.Key;
            HexSquare sq = kvp.Value;

            u.initUnit(sq, Instantiate(hpBarPrefab).GetComponent<HealthBarManager>(), hpCanvas);
        }
    }

    public Dictionary<Unit, HexSquare> getUnitLayout() {
        return this.layout;
    }

    public bool hasStarLevel(string unitName, int starLevel) {
        return this.getStarLevel(unitName, starLevel) != null;
    }

    public Unit getStarLevel(string unitName, int starLevel) {
        foreach(Unit u in this.layout.Keys) {
            if(u.getName() == unitName  && u.getMergeLevel() == starLevel) {
                return u;
            }
        }
        return null;
    }

    //attempts to activate all traits, stores all traits that were activated
    //TODO: SOMETHING SHOULD CALL THIS!
    public List<Trait> getActiveTraits() {
        List<Trait> active = new List<Trait>();
        foreach(Trait t in this.traitCount.Keys) {
            if(t.attemptActivate(this.traitCount[t])) {
                active.Add(t);
            }
        }

        return active;
    }


    private string jsonify() {
        //future thing to create, take a layout return as string ezpz
        return "";
    }

}
