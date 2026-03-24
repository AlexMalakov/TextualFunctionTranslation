using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trait : MonoBehaviour
{
    public string name;
    public int minimumForActivate;

    public Trait(string name, int minimumForActivate) {

    }

    public int getMinimumForActive() {
        return this.minimumForActivate;
    }

    public bool attemptActivate(int numUnits) {
        if(numUnits < this.minimumForActivate) {
            return false;
        }

        return true;

        //TODO: ELSE DO STUFF !!!!!
    }

    //this should do whatever the trait does
    public abstract void doThing();


}
