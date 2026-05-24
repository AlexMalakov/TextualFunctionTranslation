using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> attackedUnits;



    public override void onPrecombatStart() {
        this.attackedUnits = new Dictionary<Unit, List<Unit>>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.attackedUnits.Add(traitHaver, new List<Unit>());
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        List<Unit> scoutsStuns = this.attackedUnits[traitHaver];

        if(true && !scoutsStuns.Contains(target)) { //TODO check stuns for units
            //TODO: stun target
            //TODO: bonus damage to target
            //TODO: record the stun

            attackedUnits[traitHaver].Add(target);
            this.logSquare(target.getPosition());
        }
    }

    protected override string getTraitName() {
        return "scout";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

}
