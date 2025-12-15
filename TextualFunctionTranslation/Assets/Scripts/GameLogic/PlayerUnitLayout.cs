using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct USPair
{
    public Unit unit;
    public HexSquare hex;
}

public class PlayerUnitLayout : MonoBehaviour
{
    private Dictionary<Unit, HexSquare> layout;

    [SerializeField] private List<USPair> startingLayout;

    public void Awake() {
        layout = new Dictionary<Unit, HexSquare>();
        foreach(USPair u in this.startingLayout) {
            this.layout.Add(u.unit, u.hex);
        }
    }
    
    public void updateLayout(Unit u, HexSquare square) {
        if(this.layout.ContainsKey(u)) {
            this.layout[u] = square;
        } else {
            this.layout.Add(u, square);
        }
    }

    public void removeUnitFromLayout(Unit u) {
        this.layout.Remove(u);
    }

    public void placeUnits() {
        foreach (KeyValuePair<Unit, HexSquare> kvp in this.layout) {
            Unit u = kvp.Key;
            HexSquare sq = kvp.Value;

            u.setPosition(sq);
        }
    }


    private string jsonify() {
        //future thing to create, take a layout return as string ezpz
        return "";
    }

}
