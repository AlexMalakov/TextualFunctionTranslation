using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AxialCoord {
    public int q, r, s;

    public AxialCoord(int q, int r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
    }

    public OffsetCoord toOffset() {
        int parity = r&1;
        x = q + (r - parity) / 2
        y = r;
        return new OffsetCoord(x, y);
    }
}

public struct OffsetCoord {
    public int x, y;

    public OffsetCoord(int x, int y) {
        this.x = x;
        this.y = y;

        this.parity = y&1;
    }

    public AxialCoord toAxial() {
        int parity = y&1;
        int q = x - (y - parity) / 2;
        int r = y;
        return new AxialCoord(q, r);
    }
}

public class HexSquare : MonoBehaviour
{
    private int x, y;
    private AxialCoord axc;
    
    public void init(int x, int y, Vector3 hexPos) {
        this.x = x; this.y = y;
        this.transform.position = hexPos;
        this.axc = new OffsetCoords(x, y).toAxial();
    }

    public List<AxialCoord> getNeighborCoords() {
        return new List<AxialCoord>(){
            new AxialCoord(this.axc.q+1, this.axc.r),
            new AxialCoord(this.axc.q-1, this.axc.r),
            new AxialCoord(this.axc.q, this.axc.r+1),
            new AxialCoord(this.axc.q, this.axc.r-1),
            new AxialCoord(this.axc.q+1, this.axc.r-1),
            new AxialCoord(this.axc.q-1, this.axc.r+1)};
    }

    public AxialCoord getCoords() {
        return this.axc;
    }

    //returns distance between two hexSquares
    public static int hexDist(HexSquare s1, HexSquare s2) {
        AxialCoord s1c = s1.getCoords();
        AxialCoord s2c = s2.getCoords();
        return s1c.q - s2c.q + s1c.r - s2c.r + s1c.s - s2c.s;
    }
}
