using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    //TODO: make this unit do something in combat????
    public TrackerTrait() : base("tracker", 1) {}


    public override void onCombatEnd(Unit traitHaver) {
        //TODO: give player 1 oracle lense
    }

}
