using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//struct linking a unit to a position
[Serializable]
public struct USPair
{
    public Unit unit;
    public OffsetCoord coord;

    public USPair(Unit u, OffsetCoord c) {
        unit = u; coord = c;
    }
}

//stores the units a player has on the board: their position and the number of each trait that is present
public class PlayerUnitLayout : MonoBehaviour
{
    private Dictionary<Unit, HexSquare> layout;
    private Dictionary<Unit, HexSquare> backupLayout;

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
            this.backupLayout = new Dictionary<Unit, HexSquare>(this.layout);
            return;
        }

        this.layout = new Dictionary<Unit, HexSquare>();
        foreach(USPair u in this.startingLayout) {
            Debug.Log("MAYBE POST LAYOUT FAKING!!!!! ADDING UNIT " + u.unit.getName());
            this.layout.Add(u.unit, this.board.getHexAt(u.coord));
            this.addTrait(u.unit);
        }

        this.backupLayout = new Dictionary<Unit, HexSquare>(this.layout);
        this.traitDisplay.onUnitLayoutUpdate();
    }

    public void fakeInitLayout(Dictionary<Unit, HexSquare> fakeLayout) {
        Debug.Log("LAYOUT FAKING!!!!!");
        if(this.layout != null) {
            for(int i = this.layout.Keys.Count - 1; i >= 0; i--) {
                Unit u = this.layout.Keys.ToList()[i];
                this.removeUnitFromLayout(u);
                u.unclaimSquare();
            }
        }
        this.layout = null;
        List<USPair> fakeLayouttt = new List<USPair>();
        foreach(Unit u in fakeLayout.Keys) {
            Debug.Log("ADDING TO FAKE LAYOUT: " + u.getName());
            fakeLayouttt.Add(new USPair(u, fakeLayout[u].getCoords().toOffset()));
        }
        this.startingLayout = fakeLayouttt;
        this.initLayout();
    }


    //given a unit, add the role trait and class trait to the traits dic
    private void addTrait(Unit u) {
        Trait rt = u.getRoleTrait();
        rt.registerUnit(true, u);

        Trait ct = u.getClassTrait();
        ct.registerUnit(true, u);
    }

    //given a unit, remove its role and class traits from the dict
    private void removeTrait(Unit u) {
        Trait rt = u.getRoleTrait();
        rt.registerUnit(false, u);

        Trait ct = u.getClassTrait();
        ct.registerUnit(false, u);
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

            if(this.humanPlayer.getBench().unitInBench(u)) { //unit from bench
                //remove unit from the bench,
                this.humanPlayer.getBench().removeUnitFromBench(u);

            } //for shop, unit needs to be instantiated already

            if(square.getUnit() != null) { //swap this unit to the bench
                //INVARIANT: THERE SHOULD BE ROOM ON THE BENCH: TODO: DEBUG IN THE EVENT THAT THERE ISN"T YOU SMILE
                Unit swapper = square.getUnit();
                swapper.unclaimSquare();
                this.removeUnitFromLayout(swapper);
                BenchSlot dest = this.humanPlayer.getBench().addUnitToBench(swapper);
                StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(swapper, dest.transform, false));
            }
            
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
        if(this.layout.ContainsKey(u)) {
            this.layout.Remove(u);
            this.removeTrait(u);
            // this.traitDisplay.onUnitLayoutUpdate();
        }
    }

    public List<Unit> getActiveUnits() {
        return new List<Unit>(this.layout.Keys);
    }

    public void placeUnits() {
        foreach (KeyValuePair<Unit, HexSquare> kvp in this.layout) {
            Unit u = kvp.Key;
            u.unclaimSquare();
        }

        foreach (KeyValuePair<Unit, HexSquare> kvp in this.layout) {
            Unit u = kvp.Key;
            HexSquare sq = kvp.Value;

            if(this.humanPlayer != null) {
                this.humanPlayer.getUnitMoveManager().overrideMovement(u);
            }
            u.initUnit(sq);
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
        foreach(Trait t in this.player.getAllTraits()) {
            if(t.attemptActivate(t.getNumberActive())) {
                active.Add(t);
            }
        }

        return active;
    }

    private string jsonify() {
        //future thing to create, take a layout return as string ezpz
        return "";
    }


    public void resetToCombatStart() {
        this.layout = new Dictionary<Unit, HexSquare>(this.backupLayout);
    }

    public int countRandomizers() {
        int live = 0;
        foreach(Unit u in this.layout.Keys) {
            if(u is Randomizer) {
                live++;
            }
        }
        return live;
    }
}
