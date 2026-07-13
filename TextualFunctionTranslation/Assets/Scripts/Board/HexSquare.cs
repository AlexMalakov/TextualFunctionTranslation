using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public struct AxialCoord {
    public int q, r, s;

    public AxialCoord(int q, int r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
    }

    public AxialCoord(int q, int r, int s) {
        if(q + r + s != 0) {
            throw new Exception("INVALID Q R S VALUES!");
        }
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public OffsetCoord toOffset() {
        int parity = r&1;
        int x = q + (int)Mathf.Floor((r + parity) * .5f);
        int y = r;
        return new OffsetCoord(x, y);
    }

    public AxialCoord getMirroredCoord() {
        int mirroredQ = 4 - this.q;
        int mirroredR = 7 - this.r;
        int mirroredS = -(mirroredR + mirroredQ);

        return new AxialCoord(mirroredQ, mirroredR, mirroredS);
    }
}

[Serializable]
public struct OffsetCoord {
    public int x, y;

    public OffsetCoord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public AxialCoord toAxial() {
        int parity = y&1;
        int q = x - (int)Mathf.Floor((y + parity) * .5f);
        int r = y;
        return new AxialCoord(q, r);
    }
}

public class HexSquare : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Renderer render;
    private AxialCoord axc;
    private Unit unit;
    private Color defaultColor;

    private char characterValue;
    
    public virtual void init(int x, int y, Vector3 hexPos) {
        this.text.gameObject.SetActive(false);
        this.transform.position = hexPos;
        this.axc = new OffsetCoord(x, y).toAxial();
        defaultColor = this.render.material.color;
    }

    public virtual List<AxialCoord> getNeighborCoords() {
        return new List<AxialCoord>(){
            new AxialCoord(this.axc.q+1, this.axc.r-1, this.axc.s),
            new AxialCoord(this.axc.q+1, this.axc.r, this.axc.s-1),
            new AxialCoord(this.axc.q-1, this.axc.r+1, this.axc.s),
            new AxialCoord(this.axc.q-1, this.axc.r, this.axc.s+1),
            new AxialCoord(this.axc.q, this.axc.r+1, this.axc.s-1),
            new AxialCoord(this.axc.q, this.axc.r-1, this.axc.s+1)};
    }

    public char getCharValue() {
        return characterValue;
    }

    public virtual AxialCoord getCoords() {
        return this.axc;
    }

    public virtual bool isSquare() {
        return true;
    }

    //returns distance between two hexSquares
    public static int hexDist(HexSquare s1, HexSquare s2) {
        AxialCoord s1c = s1.getCoords();
        AxialCoord s2c = s2.getCoords();
        return (Mathf.Abs(s1c.q - s2c.q) + Mathf.Abs(s1c.r - s2c.r) + Mathf.Abs(s1c.s - s2c.s))/2;
    }

    public virtual Unit getUnit() {
        return this.unit;
    }

    public virtual void claimSquare(Unit u, bool status) {
        if(status) {
            Debug.Log("CLAIMING SQUARE " + this.toString());
            if(this.unit != null) {
                throw new Exception("cannot claim an already claimed hex square! " + u.gameObject.name + " on square " + this.toString());
            } 
            this.unit = u;
        } else {
            Debug.Log("UNCLAIMING SQUARE " + this.toString());
            if(this.unit == null) {
                throw new Exception("cannot leave an unoccupied hex square! " + u.gameObject.name + " on square " + this.toString());
            }
            this.unit = null;
        }
    }

    public void assignChar(char c) {
        //TODO: use me
        this.text.text = c + "";
    }

    public void displayValue(bool toggle) {
        this.text.gameObject.SetActive(toggle);
    }

    public void highlightSquare(Color color, float duration) {
        this.render.material.color = color;
        this.text.color = new Color(255, 0, 0);
        Invoke(nameof(unhighlight), duration);
    }
    
    private void unhighlight() {
        this.render.material.color = this.defaultColor;
        this.text.color = new Color(0, 0, 0);
    }

    public string toString() {
        // return this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y;
        return "OFFEST: " + this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y + " CUBIC: " + this.getCoords().q+ ", " + this.getCoords().r+ ", " + this.getCoords().s;
    }
}
