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
        killer.buffStat(BuffableStats.AttackSpeed, debuff, traitHaver);

        foreach(HexSquare s in squares) {
            if(s.getUnit() != null) {
                s.getUnit().buffStat(BuffableStats.AttackSpeed, debuff, traitHaver);
            }
        }
    }

    public override string getTraitName() {
        return "plague doctor";
    }

    public override string getTraitDescription() {
        return ("An order of doctors who pretected the kingdom from a deadly plague, who have now been recruited to act as combat \"medics\"."
                + " On death debuff their killer and all units in 1 square of them with" + attackSpeedLossAmount + " less attack speed. Does not stack.");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}
