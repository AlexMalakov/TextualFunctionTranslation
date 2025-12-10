using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class AStarNode {
    public HexSquare val;
    public AStarNode? prev;
    public int weight;

    public AStarNode(HexSquare val, AStarNode? prev, int weight) {
        this.val = val;
        this.prev = prev;
        this.weight = weight;
    }
}

public class AStar
{
    public static List<HexSquare> runAStar(HexSquare start, HexSquare dest, HexBoard board) {
        PriorityQueue<AStarNode, int> pq =  new PriorityQueue<HexSquare, int>();

        int insertionOrder = 0;
        pq.Enqueue(new AStarNode(start, null, 0), (0, insertionOrder));
        insertionOrder++;
        List<HexSquare> visited = new List<HexSquare>(){start};
        AStarNode workingNode = null;
        workingNode = pq.Dequeue();
        while(pq.Count > 0 && workingNode.val != dest) {
            if(visited.Contains(workingNode.val)) {
                workingNode = pq.Dequeue();
                continue;
            }

            visited.Add(workingNode.val);
            foreach(AxialCoord axc in workingNode.val.getNeighborCoords()) {
                HexSquare? neighbor = getHexAt(axc);
                if(neighbor != null && !visited.Contains(neighbor) && !board.isReserved(neighbor)) {
                    int weight = workingNode.weight + 1;
                    int heursiticWeight = weight + HexSquare.hexDist(neighbor, dest);
                    pq.Enqueue(new AStarNode(neighbor, workingNode, weight), (heursiticWeight, insertionOrder));
                    insertionOrder++;
                }
            }

            workingNode = pq.Dequeue();
        }

        List<HexSquare> pathSequence = new List<HexSquare>();
        while(workingNode != null) {
            pathSequence.Add(workingNode.val);
            workingNode = workingNode.prev;
        }

        return pathSequence;
    }
}


    