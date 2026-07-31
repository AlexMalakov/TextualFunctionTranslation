using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanTrait : InformationTrait
{
    Dictionary<Unit, int> unitCounters;
    [SerializeField] private float smallAttackGain;
    [SerializeField] private float largeAttackGain;

    public override void onPrecombatStart() {
        this.unitCounters = new Dictionary<Unit, int>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.unitCounters.Add(traitHaver, 0);
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(this.unitCounters[traitHaver] < 15) {
            float attackGain = this.getNumberActive() > 2 ? this.largeAttackGain : this.smallAttackGain;
            Operation buffOpp = new Operation(OperationType.Add, attackGain, "Marksman Shot Buff " + this.unitCounters[traitHaver]);
            traitHaver.buffStat(BuffableStats.AttackSpeed, buffOpp, traitHaver);
        } 

        this.unitCounters[traitHaver] = this.unitCounters[traitHaver] + 1;

        if(this.unitCounters[traitHaver] % 15 == 0) {
            this.logSquare(target.getPosition());
        }
    }

    public override string getTraitName() {
        return "marksman";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}
