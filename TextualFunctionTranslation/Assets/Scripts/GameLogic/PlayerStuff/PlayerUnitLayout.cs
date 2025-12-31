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

    
    private List<Unit> draggedUnits = new List<Unit>();

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
                this.layout[swapper] = this.layout[u];
                swapper.placeUnitOnSquare(this.layout[u]);
                StartCoroutine(this.dragUnitToBoard(swapper, this.layout[u]));
            } 
            this.layout[u] = square;
        } else {
            if(this.player.getMaxCombatUnits() <= this.layout.Count && (square.getUnit() == null || !this.player.getBench().canAddUnitToBench())) {
                return false;//TODO: move unit back to the bench
            }

            if(square.getUnit() != null) {
                Unit swapper = square.getUnit();
                this.layout.Remove(swapper);
                int dest = this.player.getBench().addUnitToBench(swapper);

                StartCoroutine(this.player.getBench().dragUnitToBench(u, dest));
            }
            this.layout.Add(u, square);
        }
        u.placeUnitOnSquare(square);
        StartCoroutine(this.dragUnitToBoard(u, square));
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

    //can drag one or two units
    public IEnumerator dragUnitToBoard(Unit u1, HexSquare dest) {
        if(this.draggedUnits.Contains(u1)) { //is this possible? {
            yield break;
        }
        this.draggedUnits.Add(u1);

        Vector3 startPos = u1.transform.position;
        Vector3 endPos = dest.transform.position;

        float elapsed = 0f;
        float totalDur = (endPos - startPos).magnitude / this.player.getUnitDragSpeed();

        while(elapsed < totalDur) {

            if (!this.draggedUnits.Contains(u1)) {
                yield break;
            }

            u1.transform.position = Vector3.Lerp(startPos, endPos, elapsed/totalDur);
            elapsed += Time.time;
            yield return null;
        }

        this.draggedUnits.Remove(u1);
    }

    public void unitClickedOn(Unit u) {
        this.draggedUnits.Remove(u);
    }

    private string jsonify() {
        //future thing to create, take a layout return as string ezpz
        return "";
    }

}
