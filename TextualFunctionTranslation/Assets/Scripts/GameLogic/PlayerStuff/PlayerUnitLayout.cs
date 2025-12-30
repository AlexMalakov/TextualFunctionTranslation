using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct USPair
{
    public Unit unit;
    public OffsetCoord coord;
}

public class PlayerUnitLayout : MonoBehaviour
{
    private Dictionary<Unit, HexSquare> layout;

    [SerializeField] private List<USPair> startingLayout;
    [SerializeField] private Player player;
    [SerializeField] private HexBoard board;

    public void initLayout() {
        this.layout = new Dictionary<Unit, HexSquare>();
        foreach(USPair u in this.startingLayout) {
            this.layout.Add(u.unit, this.board.getHexAt(u.coord));
        }
    }
    
    //return false if fails
    //this is ugly as shit but womp womp
    public bool attemptUpdateLayout(Unit u, HexSquare square) {
        //TODO: check if unit and square can be used for layout
        //TODO: move units to new positions
        if(this.layout.ContainsKey(u)) {
            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout[swapper] = this.layout[unit];
                swapper.placeUnitOnSquare(this.layout[unit]);
            } 
            this.layout[u] = square;
        } else {
            if(this.player.getMaxCombatUnits() <= this.layout.Count && (square.getUnit() == null || !this.player.getBench().canAddUnitToBench())) {
                return false;//TODO: move unit back to the bench
            }

            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout.Remove(swapper);
                this.player.getBench.addUnitToBench(swapper);
            }
            this.layout.Add(u, square);
        }
        u.placeUnitOnSquare(square);
        return true;
    }

    public void removeUnitFromLayout(Unit u) {
        this.layout.Remove(u);
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


    private string jsonify() {
        //future thing to create, take a layout return as string ezpz
        return "";
    }

}
