using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueDoctorTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private ParticleManager pm;
    [SerializeField] private float attackSpeedLossAmount;

    public override void onUnitDeath(Unit traitHaver, Unit killer) {
        Debug.Log(traitHaver.getName() + "I AM DEAD AND I WAS KILLED BY " + killer.getName());
        this.log.logEvent(new CombatEvent(traitHaver, killer, CombatEventType.Death));
        List<HexSquare> squares = this.getSquaresInRadius(killer.getPosition(), 1, cm.getBoard());

        Operation debuff = new Operation(OperationType.PostAdd, -this.attackSpeedLossAmount, "Doctor Debuff", true);
        killer.buffStat(BuffableStats.AttackSpeed, debuff, traitHaver);

        this.logSquare(killer.getPosition());
        this.pm.requestParticle(killer.getPosition(), ParticleType.Fart, 1.5f);

        Debug.Log(traitHaver.getName() + "I AM DEAD AND THIS MANY HEXES SHOULD GET FARTED ON:" + squares.Count);

        foreach(HexSquare s in squares) {
            if(s.isSquare() && s.getUnit() != null) {
                s.getUnit().buffStat(BuffableStats.AttackSpeed, debuff, traitHaver);
            }

            if(s.isSquare()) {
                this.logSquare(s);
                this.pm.requestParticle(s, ParticleType.Fart, 1.5f);
            }
        }
    }

    public override string getTraitName() {
        return "plague doctor";
    }

    public override string getTraitNarrative() {
        return "Unethical medics who are more concerned with spreading disease across the battle field than healing thier fellow soldiers.";
    }

    public override string getTraitEffect() {
        return (
            "On death, debuff their killer and all surrounding units. Debuffed units lose " + attackSpeedLossAmount + " attack speed. Does not stack."
        );
    }

    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}