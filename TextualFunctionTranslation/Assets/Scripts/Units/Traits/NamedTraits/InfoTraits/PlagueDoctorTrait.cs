using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueDoctorTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float attackSpeedLossAmount;

    public override void onUnitDeath(Unit traitHaver, Unit killer) {
        List<HexSquare> squares = this.getSquaresInRadius(killer.getPosition(), 1, cm.getBoard());


        Operation debuff = new Operation(OperationType.PostAdd, this.attackSpeedLossAmount, "Doctor Debuff");
        killer.buffStat(BuffableStats.AttackSpeed, debuff);

        foreach(HexSquare s in squares) {
            if(s.getUnit() != null) {
                s.getUnit().buffStat(BuffableStats.AttackSpeed, debuff);
            }
        }
    }

    public override string getTraitName() {
        return "doctor";
    }

    protected override int getMinimumForActive() {
        return 2;
    }
}
