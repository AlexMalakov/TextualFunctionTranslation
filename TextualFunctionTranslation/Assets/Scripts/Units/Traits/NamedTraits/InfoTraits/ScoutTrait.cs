using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    private List<Unit> attackedUnits;

    public ScoutTrait() : base("scout", 2) {}


    public override void onPrecombatStart() {

    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(true && !attackedUnits.Contains(target)) { //TODO check stuns for units
            //TODO: stun target
            //TODO: bonus damage to target
            //TODO: record the stun

            attackedUnits.Add(target);

            this.logSquare(target.getPosition());
        }
    }

}
