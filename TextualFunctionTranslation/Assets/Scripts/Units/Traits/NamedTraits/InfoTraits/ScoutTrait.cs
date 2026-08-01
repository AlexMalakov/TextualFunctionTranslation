using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> attackedUnits;

    [SerializeField] private float stunDamageSmall;
    [SerializeField] private float stunDamageLarge;

    [SerializeField] private float stunDuration;


    public override void onPrecombatStart() {
        this.attackedUnits = new Dictionary<Unit, List<Unit>>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.attackedUnits.Add(traitHaver, new List<Unit>());
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        List<Unit> scoutsStuns = this.attackedUnits[traitHaver];

        if(!scoutsStuns.Contains(target)) {
            target.stunUnit(this.stunDuration);
            
            float damage = this.getNumberActive() > 2 ? this.stunDamageLarge : this.stunDamageSmall;
            target.takeDamage(damage, traitHaver);

            attackedUnits[traitHaver].Add(target);
            this.logSquare(target.getPosition());
        }
    }

    public override string getTraitName() {
        return "scout";
    }

    public override string getTraitDescription() {
        return ("The bravest and fastest, acting as an warning system for the rest of the party. When a scout attacks a unit for the first time, "
            + "their opponent is stunned for " + stunDuration + " seconds, and takes " + stunDamageSmall + "/" + stunDamageLarge + " damage. Units can be stunned"
            + " ny multiple different scounts, but only once per scout.");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}
