using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    //TODO: make this unit do something in combat????


    public override void onCombatEnd(Unit traitHaver) {
        //TODO: give player 1 oracle lense
    }

    public override string getTraitName() {
        return "tracker";
    }

    protected override int getMinimumForActive() {
        return 1;
    }

}
