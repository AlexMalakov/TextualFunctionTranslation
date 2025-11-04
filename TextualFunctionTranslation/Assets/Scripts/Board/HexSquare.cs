using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSquare : MonoBehaviour
{
    private int x, private int y;
    
    public void init(int x, int y, Vector3 hexPos) {
        this.x = x; this.y = y;
        this.transform.position = hexPos;
    }

    //returns distance between two hexSquares
    public static int hexDist(HexSquare s1, HexSquare s2) {
        //TODO: implement
        return 0;
    }
}
