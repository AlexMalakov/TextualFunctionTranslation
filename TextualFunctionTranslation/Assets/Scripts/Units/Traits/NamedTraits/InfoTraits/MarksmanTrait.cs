using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanTrait : InformationTrait
{
    Dictionary<Unit, int> unitCounters;
    [SerializeField] private float attackSpeedGain;


    public override void onPrecombatStart() {
        base.onPrecombatStart();
        this.unitCounters = new Dictionary<Unit, int>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.unitCounters.Add(traitHaver, 0);
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(this.unitCounters[traitHaver] < 15) {
            Operation buffOpp = new Operation(OperationType.PostAdd, this.attackSpeedGain, "Marksman Shot Buff " + this.unitCounters[traitHaver]);
            traitHaver.buffStat(BuffableStats.AttackSpeed, buffOpp);
        } 

        this.unitCounters[traitHaver] = this.unitCounters[traitHaver] + 1;

        if(this.unitCounters[traitHaver] % 15 == 0) {
            this.logSquare(target.getPosition());
        }
    }

    protected override string getTraitName() {
        return "marksman";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

}
