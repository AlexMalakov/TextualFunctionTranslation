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
        if(!this.board.getPlayerSqures().Contains(square) || !this.player.ownsUnit(u)) {
            return false;
        }
        if(this.layout.ContainsKey(u)) {
            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout[swapper] = this.layout[u];
                swapper.placeUnitOnSquare(this.layout[u]);
                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, this.layout[u].transform, false));
            } 
            this.layout[u] = square;
        } else {
            if(this.player.getMaxCombatUnits() <= this.layout.Count && (square.getUnit() == null || !this.player.getBench().canAddUnitToBench())) {
                return false;
            }

            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout.Remove(swapper);
                BenchSlot dest = this.player.getBench().addUnitToBench(swapper);

                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, dest.transform, false));
            }
            this.layout.Add(u, square);
        }
        u.placeUnitOnSquare(square);
        StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, square.transform, true));
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
