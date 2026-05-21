using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperationType {
    PreMultiply = 0, Add = 1, PostMultiply = 2, PostAdd = 3
}

struct Operation {
    OperationType type;
    float amount;

    Operation(OperationType type, float amount) {
        this.type = type;
        this.amount = amount;
    }

    float apply(float incoming) {
        if(this.type == OperationType.PreMultiply or this.type == OperationType.PostMultiply) {
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

    public void addOperation(OperationType type, float value) {
        this.operations.Add(new Operation(type, value));
    }

    public void wipe() {
        this.operations = new List<Operation>();
    }

    public float calculate() {
        this.operations.Sort((a, b) => a.type.CompareTo(b.type));

        float carry = this.val;
        for(Operation o in this.operations) {
            carry = o.apply(carry);
        }

        return carry;
    }
}
