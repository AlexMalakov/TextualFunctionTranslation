using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanTrait : InformationTrait
{
    Dictionary<Unit, int> unitCounters;
    public MarksmanTrait() : base("marksman", 2) {}


    public virtual void onPrecombatStart() {
        this.unitCounters = new Dictionary<Unit, int>();
    }

    public virtual void onCombatStart(Unit traitHaver) {
        this.unitCounters.Add(traitHaver, 0);
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(this.unitCounters[traitHaver] < 15) {
            //TODO give the unit it's stat bonus

        } 

        this.unitCounters[traitHaver] = this.unitCounters[traitHaver] + 1;

        if(this.unitCounters[traitHaver] % 15 == 0) {
            this.logSquare(target.getPosition());
        }
    }


}
