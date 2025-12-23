using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private AxialCoord axc;
    private Unit unit;
    
    public virtual void init(int x, int y, Vector3 hexPos) {
        this.transform.position = hexPos;
        this.axc = new OffsetCoord(x, y).toAxial();
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
            if(this.unit != null) {
                throw new Exception("cannot claim an already claimed hex square!");
            } 
            this.unit = u;
        } else {
            if(this.unit == null) {
                throw new Exception("cannot leave an unoccupied hex square!");
            }
            this.unit = null;
        }
    }

    public string toString() {
        // return this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y;
        return "OFFEST: " + this.getCoords().toOffset().x + ", " + this.getCoords().toOffset().y + " CUBIC: " + this.getCoords().q+ ", " + this.getCoords().r+ ", " + this.getCoords().s;
    }
}
