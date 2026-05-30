using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> attackedUnits;

    [SerializeField] private float stunDamageSmall;
    [SerializeField] private float stunDamageLarge;


    public override void onPrecombatStart() {
        base.onPrecombatStart();
        this.attackedUnits = new Dictionary<Unit, List<Unit>>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.attackedUnits.Add(traitHaver, new List<Unit>());
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        List<Unit> scoutsStuns = this.attackedUnits[traitHaver];

        if(!scoutsStuns.Contains(target)) {
            //TODO: stun target
            
            float damage = this.getNumberActive() > 2 ? this.stunDamageLarge : this.stunDamageSmall;
            target.takeDamage(damage);

            attackedUnits[traitHaver].Add(target);
            this.logSquare(target.getPosition());
        }
    }

    public override string getTraitName() {
        return "scout";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

}
