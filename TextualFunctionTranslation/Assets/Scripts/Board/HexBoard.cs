using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBoard : MonoBehaviour
{
    List<List<HexSquare>> squares = new List<List<HexSquare>>();
    [SerializeField] private GameObject hexSquarePrefab;

    [Header ("object placement")]
    [SerializeField] float xoffset;
    [SerializeField] float yoffset;
    


    //create a hex board that will be used
    public void Awake() {
        for(int y = 0; y < 8; y++) {
            List<HexSquare> lis = new List<HexSquare>();
            for(int x = 0; x < ((y % 2 == 0) ? 6 : 7); x++) {
                lis.Add(Instantiate(this.hexSquarePrefab).GetComponent<HexSquare>());
                lis[lis.Count - 1].init(x,y, calculateHexPos(x, y));
            }
            squares.Add(lis);
        }
    }


    public HexSquare? pathfindToSquare(HexSquare source, HexSquare dest) {
        return null;
    }

    private Vector3 calculateHexPos(int x, int y) {
        return new Vector3(
            this.transform.position.x + ((y % 2 == 0) ? this.xoffset/2 : 0) + x * this.xoffset,
            this.transform.position.y,
            this.transform.position.z - y * this.yoffset
        );
    }

    public List<Square> filterOperationOnSquares(Func<Square, bool> filterOperation) {
        List<Square> res = new List<Square>();

        foreach(List<Square> sqrs in this.squares) {
            foreach(Square s in sqrs) {
                if(filterOperation(s)) {
                    res.Add(s);
                }
            }
        }

        return res;
    }
}
