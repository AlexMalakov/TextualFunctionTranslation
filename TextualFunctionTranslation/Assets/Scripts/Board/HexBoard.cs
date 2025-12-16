using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexBoard : MonoBehaviour
{
    List<List<HexSquare>> squares = new List<List<HexSquare>>();
    [SerializeField] private GameObject hexSquarePrefab;
    [SerializeField] private NullSquare nullSQ;

    [Header ("object placement")]
    [SerializeField] float xoffset;
    [SerializeField] float yoffset;
    


    //create a hex board that will be used
    public void initBoard() {
        for(int y = 0; y < 8; y++) {
            List<HexSquare> lis = new List<HexSquare>();
            for(int x = 0; x < ((y % 2 == 0) ? 6 : 7); x++) {
                lis.Add(Instantiate(this.hexSquarePrefab).GetComponent<HexSquare>());
                lis[lis.Count - 1].init(x,y, calculateHexPos(x, y));
            }
            this.squares.Add(lis);
        }
    }

    public HexSquare getHexAt(AxialCoord c) {
        return this.getHexAt(c.toOffset());
    }

    public HexSquare getHexAt(OffsetCoord c) {
        if(this.squares.Count > c.y && c.y >= 0 && this.squares[c.y].Count > c.x && c.x >= 0) {
            return this.squares[c.y][c.x];
        }
        return this.nullSQ;
    }

    //returns null if path does not exist
    
    public HexSquare pathfindToSquare(HexSquare source, HexSquare dest) {
        Debug.Log("DOING ROUTE TO FROM" + source.toString() + " | " + dest.toString());
        List<HexSquare> path = AStar.runAStar(source, dest, this);
        return path.Count > 2 ? path[path.Count-2] : this.nullSQ;
    }

    private Vector3 calculateHexPos(int x, int y) {
        return new Vector3(
            this.transform.position.x + ((y % 2 == 0) ? this.xoffset/2 : 0) + x * this.xoffset,
            this.transform.position.y,
            this.transform.position.z - y * this.yoffset
        );
    }

    public List<HexSquare> filterOperationOnSquares(Func<HexSquare, bool> filterOperation) {
        List<HexSquare> res = new List<HexSquare>();

        foreach(List<HexSquare> sqrs in this.squares) {
            foreach(HexSquare s in sqrs) {
                if(filterOperation(s)) {
                    res.Add(s);
                }
            }
        }

        return res;
    }
}
