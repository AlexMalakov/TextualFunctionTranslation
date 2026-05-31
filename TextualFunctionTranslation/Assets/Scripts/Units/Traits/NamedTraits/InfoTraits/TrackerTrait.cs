using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> assistMap;
    private int lenseCount;

    public override void onPrecombatStart() {
        this.lenseCount = 0;
        this.assistMap = new Dictionary<Unit, List<Unit>>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.assistMap.Add(traitHaver, new List<Unit>());
    }

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
