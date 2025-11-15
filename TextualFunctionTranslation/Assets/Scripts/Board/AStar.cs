using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct Node {
    public HexSquare val;
    public Node? prev;
    public int weight;

    public Node(HexSquare val, Node? prev, int weight) {
        this.val = val;
        this.prev = prev;
        this.weight = weight;
    }
}

public class AStar
{
    public static List<HexSquare> runAStar(HexSquare start, HexSquare dest, HexBoard board) {
        PriorityQueue<Node, int> pq =  new PriorityQueue<HexSquare, int>();

        int insertionOrder = 0;
        pq.Enqueue(new Node(start, null, 0), (0, insertionOrder));
        insertionOrder++;
        List<HexSquare> visited = new List<HexSquare>(){start};
        Node workingNode = null;
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
                    pq.Enqueue(new Node(neighbor, workingNode, weight), (heursiticWeight, insertionOrder));
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

struct Node {
    public HexSquare square;
    public HexSquare prev;
}


    