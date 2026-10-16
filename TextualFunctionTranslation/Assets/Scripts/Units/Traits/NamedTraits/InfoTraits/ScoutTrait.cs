using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> attackedUnits;

    [SerializeField] private float stunDamageSmall;
    [SerializeField] private float stunDamageLarge;

    [SerializeField] private float healSmall;
    [SerializeField] private float healLarge;

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
            this.log.logEvent(new CombatEvent(traitHaver, target, CombatEventType.Attack));
            this.logSquare(target.getPosition());
            target.stunUnit(this.stunDuration);
            
            float damage = this.getNumberActive() > 3 ? this.stunDamageLarge : this.stunDamageSmall;
            target.takeDamage(damage, traitHaver);

            attackedUnits[traitHaver].Add(target);
        }
    }


    public override void onReposition(Unit traitHaver) {
        traitHaver.heal(traitHaver.calcMaxHealth() * (this.getNumberActive() > 3 ? this.healLarge : this.healSmall) / 100f);
        this.logSquare(traitHaver.getPosition());
    }

    public override string getTraitName() {
        return "scout";
    }

    public override string getTraitNarrative() {
        return "The nimblest soldiers in the party, who regularly make first contact with opposing forces.";
    }

    public override string getTraitEffect() {
        return (
            "When a scout attacks a unit for the first time, the target is stunned for " + stunDuration + " seconds, and takes "
            + stunDamageSmall+"/"+stunDamageLarge + ". Everytime a scout moves to, they heal for " + this.healSmall+"/"+this.healLarge + " percent max health." 
        );
    }

    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}
