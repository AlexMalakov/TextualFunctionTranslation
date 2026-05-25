using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperationType {
    PreAdd = 0, PreMultiply = 1, Add = 2, PostMultiply = 3, PostAdd = 4
}

public struct Operation {
    public OperationType type;
    public float amount;
    public string buffName;

    public Operation(OperationType type, float amount, string buffName) {
        this.type = type;
        this.amount = amount;
        this.buffName = buffName;
    }

    public float apply(float incoming) {
        if(this.type == OperationType.PreMultiply || this.type == OperationType.PostMultiply) {
            return this.amount * incoming;
        }
        return this.amount + incoming;
    }
}

public class UnitNumericCalculator
{
    private float val;
    private List<Operation> operations;

    public UnitNumericCalculator(float val) {
        this.val = val;
        this.operations = new List<Operation>();
    }

    public void addOperation(Operation newOperation) {
        foreach(Operation o in this.operations) {
            if(newOperation.buffName == o.buffName) {
                return;
            }
        }
        this.operations.Add(newOperation);
    }

    public void wipe() {
        this.operations = new List<Operation>();
    }

    public float calculate() {
        this.operations.Sort((a, b) => a.type.CompareTo(b.type));

        float carry = this.val;
        foreach(Operation o in this.operations) {
            carry = o.apply(carry);
        }

        return carry;
    }
}
