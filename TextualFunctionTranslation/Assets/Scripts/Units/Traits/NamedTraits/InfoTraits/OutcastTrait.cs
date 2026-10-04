using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcastTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float attackSpeedBoost;
    [SerializeField] private float healthBoost;
    [SerializeField] private float damageBoost;
    //doesn't require unit count checkl

    //NOTE: Also triggers for enemies!

    public override void onCombatStart(Unit traitHaver) {
        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Start));
        List<HexSquare> circle = this.getSquaresInRadius(traitHaver.getPosition(), 1, cm.getBoard());
        foreach(HexSquare s in circle) {
            if(s.getUnit() != null)
                return;
        }

        Operation speedBoost = new Operation(OperationType.PostAdd, this.attackSpeedBoost, "Outcast Start Buff", true);
        Operation healthBoost = new Operation(OperationType.PostMultiply, this.healthBoost, "Outcast Start Buff", true);
        Operation damageBoost = new Operation(OperationType.PostMultiply, this.damageBoost, "Outcast Start Buff", true);
        traitHaver.buffStat(BuffableStats.AttackSpeed, speedBoost, traitHaver);
        traitHaver.buffStat(BuffableStats.Health, healthBoost, traitHaver);
        traitHaver.buffStat(BuffableStats.Damage, damageBoost, traitHaver);

        foreach(HexSquare s in circle) {
            this.logSquare(s);
        }
    }

    public override string getTraitName() {
        return "outcast";
    }


    public override string getTraitNarrative() {
        return "The oddballs on the fringes of the kingdoms society.";
    }

    public override string getTraitEffect() {
        return (
            "When not next to any units on combat start, gain "
            + this.healthBoost + " health, " + this.damageBoost + " damage, and" 
            + this.attackSpeedBoost + " atttack speed."
        );
    }
    

    public override int getMinimumForActive() {
        return 2;
    }
  
    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}
