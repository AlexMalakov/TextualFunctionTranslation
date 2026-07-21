using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this might be super overengineered
public abstract class UnitActionState : MonoBehaviour
{
    protected int frameLockout;
    private Unit unitOwner;

    protected abstract void onStateEnter(UnitActionState stateFrom);

    protected abstract void onStateExit();

    protected abstract bool readyToSwap();

    protected abstract void runUpdate();

    public void update() {
        this.frameLockout--;

        this.runUpdate();
        
        if(this.readyToSwap()) {
            this.unitOwner.requestStateSwap(this);
        }
    }

    public void swapStates(UnitActionState swapTo) {
        this.onStateExit();
        swapTo.onStateEnter(this);
    }
}
