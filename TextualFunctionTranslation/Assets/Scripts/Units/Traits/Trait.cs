using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait : MonoBehaviour
{
    public string name;
    public int minimumForActivate;


    public int getMinimumForActive() {
        return this.minimumForActivate;
    }

    public void attemptActivate(int numUnits) {
        if(numUnits < this.minimumForActivate) {
            return;
        }

        //TODO: ELSE DO STUFF !!!!!
    }
}
