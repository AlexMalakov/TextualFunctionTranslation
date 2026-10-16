using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System;


class StablePriorityQueue<T> {
    private readonly List<(T item, int priority, long insertionIndex)> heap = new();
    public int Count => heap.Count;
    private long insertionIndex = 0;

    public void Enqueue(T item, int priority) {
        heap.Add((item, priority, insertionIndex++));
        heapifyUp(Count - 1);
    }


    public T Dequeue() {
        printHeap();
        T item = heap[0].item;
        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);
        heapifyDown(0);
        return item;
    }

    private void heapifyUp(int index) {
        printHeap();
        if((index-1)/2 >= 0 && index > 0 && this.gt(heap[(index-1)/2], heap[index])) {
            (T item, int priority, long order) swap = heap[index];
            heap[index] = heap[(index-1)/2];
            heap[(index-1)/2] = swap;
            heapifyUp((index-1)/2);
        }
    }

    private void heapifyDown(int index) {
        if(index < Count) {
            int smallestIndex = index;

            if((index*2) + 1 < Count &&  this.gt(heap[index], heap[(index*2) + 1]) ) {
                smallestIndex = (index*2) + 1;
            }

            if((index * 2) + 2 < Count && this.gt(heap[smallestIndex], heap[(index*2) + 2])) {
                smallestIndex = (index * 2) + 2;
            }

            if(smallestIndex != index) {
                (T item, int priority, long order) swap = heap[index];
                heap[index] = heap[smallestIndex];
                heap[smallestIndex] = swap;
                heapifyDown(smallestIndex);
            }
        }
    }

    private bool lt(
                    (T item, int priority, long order) a,
                    (T item, int priority, long order) b) {
        return gt(b, a); //cannot be equal
    }

    private bool gt(
                    (T item, int priority, long order) a,
                    (T item, int priority, long order) b) {
        if(a.priority > b.priority) {
            return true;
        }

        if(a.priority == b.priority) {
            return a.order > b.order;
        }

        return false;
    }

    private void printHeap() {
        // Debug.Log("PRINTING HEAP!");
        // for(int i = 0; i < heap.Count; i++) {
        //     string message = "node at list index: " + i + " = " + heap[i];
        //     if(i/2 > 0 && i > 0) {
        //         message = message + " | parent " + i/2 + " = " + heap[i/2];
        //     }

        //     if(2*i < heap.Count - 1) {
        //         message = message + " | child " + (2*i + 1) + " = " + heap[2*i+1];
        //     }

        //     if(2*i < heap.Count - 2) {
        //         message = message + " | child " + (2*i + 2) + " = " + heap[2*i + 2];
        //     }
        //     Debug.Log(message);
        // }
        // Debug.Log("===========================");
        // Debug.Log("===========================");
    }
}


class AStarNode {
    public HexSquare val;
    public AStarNode prev;
    public int weight;

    public AStarNode(HexSquare val, AStarNode prev, int weight) {
        this.val = val;
        this.prev = prev;
        this.weight = weight;
    }
}

public class AStar
{
    public static List<HexSquare> runAStar(HexSquare start, HexSquare dest, HexBoard board, NullSquare nullS) {
        StablePriorityQueue<AStarNode> pq =  new StablePriorityQueue<AStarNode>();


        pq.Enqueue(new AStarNode(start, null, 0), 0);
        List<HexSquare> visited = new List<HexSquare>(){};
        
        AStarNode workingNode = null;
        while(pq.Count > 0) {
            workingNode = pq.Dequeue();
            if(workingNode.val == dest) {
                break;
            }

            if(visited.Contains(workingNode.val)) {
                continue;
            }

            visited.Add(workingNode.val);
            foreach(AxialCoord axc in workingNode.val.getNeighborCoords()) {
                HexSquare neighbor = board.getHexAt(axc);
                if(neighbor.isSquare() && !visited.Contains(neighbor) && (neighbor.getUnit() == null || neighbor == dest)) {
                    // Debug.Log("I am" + workingNode.val.toString() +"ADDIGN NEIGHBOR: " + neighbor.toString());
                    int weight = workingNode.weight + 1;
                    int heursiticWeight = weight + HexSquare.hexDist(neighbor, dest);
                    pq.Enqueue(new AStarNode(neighbor, workingNode, weight), heursiticWeight);
                }
            }
        }

        if (workingNode == null || workingNode.val != dest) {
            return new List<HexSquare> { nullS };
        }

        // Debug.Log("calculated route dest to start!!!!!!!!!!!!!!!!");
        List<HexSquare> pathSequence = new List<HexSquare>();
        while(workingNode != null) {
            // Debug.Log("next node: " + workingNode.val.toString());
            pathSequence.Add(workingNode.val);
            workingNode = workingNode.prev;
        }
        // Debug.Log("===========================");
        return pathSequence;
    }
}


    