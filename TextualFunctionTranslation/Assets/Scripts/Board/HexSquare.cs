using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AxialCoords {
    public int q, r, s;

    public AxialCoords(int q, int r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
    }

    public OffsetCoords toOffset() {
        int parity = r&1;
        x = q + (r - parity) / 2
        y = r;
        return OffsetCoords(x, y);
    }
}

public struct OffsetCoords {
    public int x, y;

    public OffsetCoords(int x, int y) {
        this.x = x;
        this.y = y;

        this.parity = y&1;
    }

    public AxialCoords toAxial() {
        int parity = y&1;
        int q = x - (y - parity) / 2;
        int r = y;
        return AxialCoords(q, r);
    }
}

public class HexSquare : MonoBehaviour
{
    private int x, y;
    private AxialCoords axCoords;
    
    public void init(int x, int y, Vector3 hexPos) {
        this.x = x; this.y = y;
        this.transform.position = hexPos;
        this.axCoords = new OffsetCoords(x, y).toAxial();
    }


    public AxialCoords getCoords() {
        return this.axCoords;
    }

    //returns distance between two hexSquares
    public static int hexDist(HexSquare s1, HexSquare s2) {
        AxialCoords s1c = s1.getCoords();
        AxialCoords s2c = s2.getCoords();
        return s1c.q - s2c.q + s1c.r - s2c.r + s1c.s - s2c.s;
    }
}
