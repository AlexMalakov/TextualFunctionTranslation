using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NullSquare : HexSquare
{
    public override void init(int x, int y, Vector3 hexPos) {
        throw new Exception("cannot init a null square!");
    }

    public virtual List<AxialCoord> getNeighborCoords() {
        throw new Exception("cannot get neighbors of a null square!");
    }

    public virtual AxialCoord getCoords() {
        throw new Exception("cannot get coords of a null square!");
    }

    public virtual bool isSquare() {
        return false;
    }
}
