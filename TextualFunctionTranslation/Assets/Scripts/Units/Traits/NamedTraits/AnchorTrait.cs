using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stub for the anchor trait
public class AnchorTrait : Trait
{
    public AnchorTrait(int minimumForActivate) : base("anchor", minimumForActivate){

    }


    public override void onPrecombatStart(Unit traitHaver){}

    public override void onCombatStart(Unit traitHaver){}

    public override void onFirstAttackAgainstUnit(Unit traitHaver, Unit target){}

    public override void onUnitAttack(Unit traitHaver, Unit target){}

    public override void onEnemyDefeat(Unit traitHaver, Unit target){}

    public override void onCombatSurvival(Unit traitHaver){}

    public override void onCombatEnd(Unit traitHaver){}
}
