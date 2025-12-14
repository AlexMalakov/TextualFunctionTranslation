using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NullSquare : HexSquare
{
    public override void init(int x, int y, Vector3 hexPos) {
        throw new Exception("cannot init a null square!");
    }

    public override List<AxialCoord> getNeighborCoords() {
        throw new Exception("cannot get neighbors of a null square!");
    }

    public override AxialCoord getCoords() {
        throw new Exception("cannot get coords of a null square!");
    }

    public override bool isSquare() {
        return false;
    }
}
