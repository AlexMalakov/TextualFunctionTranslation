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
        T item = heap[0];
        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);
        heapifyDown(0);
        return item.item;
    }

    private void heapifyUp(int index) {
        if(index/2 >= 0 && index > 0 && this.gt(heap[index/2], heap[index])) {
            (T item, int priority, long order) swap = heap[index];
            heap[index] = heap[index/2];
            heap[index/2] = swap;
            heapifyUp(index/2);
        }
    }

    private void heapifyDown(int index) {
        if(index < Count) {
            int smallestIndex = index;

            if(index*2 < Count &&  this.gt(heap[index], heap[index*2]) ) {
                smallestIndex = index*2;
            }

            if((index * 2) + 1 < Count && this.gt(heap[smallestIndex], heap[(index*2) + 1])) {
                smallestIndex = (index * 2) + 1;
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

        if(heap[index1] == heap[index2]) {
            return a.order > b.order;
        }

        return false;
    }
}


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
        StablePriorityQueue<AStarNode> pq =  new StablePriorityQueue<HexSquare>(); //TODO NOT STABLE?


        pq.Enqueue(new AStarNode(start, null, 0), 0);
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
                    pq.Enqueue(new AStarNode(neighbor, workingNode, weight), heursiticWeight);
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


    