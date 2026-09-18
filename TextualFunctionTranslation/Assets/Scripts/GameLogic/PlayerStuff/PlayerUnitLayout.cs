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
    [SerializeField] private TraitDisplay traitDisplay;

    private HumanPlayer humanPlayer;

    public void Awake() {
        //this is bad design, but unfortuantley since Im implementing mono I can't use constructor or polymorphism to get around this :(
        if(player is HumanPlayer) {
            this.humanPlayer = (HumanPlayer) player;
        }
    }
     
    //initializes the layout from starting layout, and sets up initial traits
    public void initLayout() {
        if(this.layout != null) {
            return;
        }

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

        rt.registerUnit(true);

        Trait ct = u.getClassTrait();
        if(this.traitCount.ContainsKey(ct)) {
            this.traitCount[ct] += 1;
        } else {
            this.traitCount.Add(ct, 1);
        }

        ct.registerUnit(true);
    }

    //given a unit, remove its role and class traits from the dict
    private void removeTrait(Unit u) {
        
        Trait rt = u.getRoleTrait();
        this.traitCount[rt] -= 1;
        if(this.traitCount[rt] == 0) {
            this.traitCount.Remove(rt);
        }

        rt.registerUnit(false);

        Trait ct = u.getClassTrait();
        this.traitCount[ct] -= 1;
        if(this.traitCount[ct] == 0) {
            this.traitCount.Remove(ct);
        }

        ct.registerUnit(false);
    }
    
    //return false if fails
    //this is ugly as shit but womp womp
    public bool attemptUpdateLayout(Unit u, HexSquare square) { //moves unit u inot square s
        //not my unit or not my destination square, then fail the movement
        if(!this.board.getPlayerSquares().Contains(square) || !this.player.ownsUnit(u) 
            || (!this.layout.ContainsKey(u) && this.humanPlayer.getMaxCombatUnits() <= this.layout.Count
                && (square.getUnit() == null //we are at full capacity and trying to add another unit
                //    || //or we are adding a unit from the shop, but maybe that's the shops responsibility to block?
                )
            )
        ) {
            return false;
        }

        HexSquare currentSquare = u.getPosition(); //this might have side effects
        u.unclaimSquare(); //might hurt

        if(this.layout.ContainsKey(u)) { //either we're swapping 2 units already on the layout or moving a unit already on the layout into a new square
            if(square.getUnit() != null) { // this is a unit swap
                Debug.Log("UNIT LAYOUT UPDATE CASE INTERNAL SWAP");
                Unit swapper = square.getUnit();
                
                swapper.unclaimSquare();

                StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(u, this.layout[swapper].transform, true));
                StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(swapper, this.layout[u].transform, true));

                swapper.placeUnitOnSquare(this.layout[u], false);
                u.placeUnitOnSquare(this.layout[swapper], false);

                this.layout[u] = square;
                this.layout[swapper] = currentSquare;
            } else {
                Debug.Log("UNIT LAYOUT UPDATE CASE SHIFT");
                StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(u, square.transform, true));
                u.placeUnitOnSquare(square, false);
                this.layout[u] = square;
            }
        } else { //either we're adding a unit from the shop or moving a unit from the bench: If from the shop can't move it into a unit, if from the bench can do whatever
            Debug.Log("UNIT LAYOUT UPDATE CASE ADD NEW UNIT");
            if(square.getUnit() != null) { //swap this unit to the bench
                //INVARIANT: THERE SHOULD BE ROOM ON THE BENCH: TODO: DEBUG IN THE EVENT THAT THERE ISN"T YOU SMILE
                Unit swapper = square.getUnit();
                swapper.unclaimSquare();
                this.removeUnitFromLayout(swapper);
                BenchSlot dest = this.humanPlayer.getBench().addUnitToBench(swapper);
                StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(swapper, dest.transform, false));
            }
            
            if(this.humanPlayer.getBench().unitInBench(u)) { //unit from bench
                //remove unit from the bench,
                this.humanPlayer.getBench().removeUnitFromBench(u);

            } //for shop, unit needs to be instantiated already
            StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(u, square.transform, true));
            u.placeUnitOnSquare(square, false);
            this.layout[u] = square;
            this.addTrait(u);
        }

        this.traitDisplay.onUnitLayoutUpdate();
        return true;
    }

    public void addUnitFromShop(Unit u) {
        foreach(HexSquare s in this.board.getPlayerSquares()) {
            if(s.getUnit() == null) {
                this.layout.Add(u, s);
                this.addTrait(u);
                u.placeUnitOnSquare(s, true);
                return;
            }
        }
        throw new Exception("cannot place unit in layout despite player unit cap allowing it!!!");
    }

    public void addUnitFromBench(Unit u, HexSquare destination) {
        this.layout.Add(u, destination);
        this.addTrait(u);
        StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(u, destination.transform, true));
        u.placeUnitOnSquare(destination, false);
        //this does not need to unclaim bench slots the bench can do that for you
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

    public bool hasAnotherStarLevel(Unit notMe, string unitName, int starLevel) {
        return this.getAnotherStarLevel(notMe, unitName, starLevel) != null;
    }

    public Unit getAnotherStarLevel(Unit notMe, string unitName, int starLevel) {
        foreach(Unit u in this.layout.Keys) {
            if(u != notMe && u.getName() == unitName && u.getMergeLevel() == starLevel) {
                return u;
            }
        }
        return null;
    }

    //attempts to activate all traits, stores all traits that were activated
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
