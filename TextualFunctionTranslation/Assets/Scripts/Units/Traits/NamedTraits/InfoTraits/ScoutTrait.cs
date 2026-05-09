using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    public ScoutTrait() : base("scout", 2) {}



    public virtual void onFirstAttackAgainstUnit(Unit traitHaver, Unit target) {
        if(true) { //TODO check stuns for units
            //TODO: stun target
            //TODO: bonus damage to target
            //TODO: record the stun

            this.logSquare(target.getPosition());
        }
    }

}
