using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this might be super overengineered
public abstract class UnitActionState
{
    protected int frameLockout;
    protected Unit unitOwner;
    protected Unit target;

    public UnitActionState(Unit u) {
        this.unitOwner = u;
    }

    public abstract void onStateEnter(UnitActionState stateFrom, StateDataContainer data); //if the target changes then the state should abort no?

    public abstract void onStateExit();

    //Can't cancel a walk or stun, but can cancel an atatck
    public virtual bool cancelAction() {
        return false;
    }

    public virtual void forceEnd() {
        this.frameLockout = 0;
        this.cancelAction();
    }

    protected abstract bool readyToSwap();

    protected abstract void runUpdate();

    public void triggerUpdate() {
        this.frameLockout--;

        this.runUpdate();
        
        if(this.readyToSwap()) {
            this.unitOwner.requestStateSwap();
        }
    }

    public void swapStates(UnitActionState swapTo, StateDataContainer data) {
        this.onStateExit();
        swapTo.onStateEnter(this, data);
    }
}
