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
        
        List<HexSquare> circle = this.getSquaresInRadius(traitHaver.getPosition(), 1, cm.getBoard());
        foreach(HexSquare s in circle) {
            if(s.getUnit() != null)
                return;
        }

        Operation speedBoost = new Operation(OperationType.Add, this.attackSpeedBoost, "Outcast Start Buff");
        Operation healthBoost = new Operation(OperationType.PreMultiply, this.healthBoost, "Outcast Start Buff");
        Operation damageBoost = new Operation(OperationType.PreMultiply, this.damageBoost, "Outcast Start Buff");
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

    protected override int getMinimumForActive() {
        return 2;
    }
  
    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }
}
