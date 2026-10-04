using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtisansTrait : CypherTrait
{
    [SerializeField] private float healAmount;
    [SerializeField] private float damageAmountSmall;
    [SerializeField] private float damageAmount;
    [SerializeField] private float damageAmountLarge;

    public override void onEnemyDefeat(Unit traitHaver, Unit target){
        float amt = this.getNumberActive() == 1 ? this.damageAmountSmall : (this.getNumberActive() == 2 ? this.damageAmount : this.damageAmountLarge);
        Operation buff = new Operation(OperationType.PreAdd, amt, "Artisan Stack for " + target.getName(), true);
        traitHaver.buffStat(BuffableStats.Damage, buff, traitHaver);
        traitHaver.heal(healAmount);

        this.log.logEvent(new CombatEvent(traitHaver, target, CombatEventType.Kill));

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();
        cypher.Add(new CharacterTransformation(target.getPosition(), traitHaver.getPosition()));
        cypher.Add(new CharacterTransformation(traitHaver.getPosition(), target.getPosition()));

        this.addToCypher(cypher);
    }

    public override string getTraitName() {
        return "artisan";
    }


    public override string getTraitNarrative() {
        return "Talented craftsman and artists whose skill with brush and chisel is only rivaled by their skill with bow and magic.";
    }

    public override string getTraitEffect() {
        return "On kill gain " + damageAmountSmall +"/"+damageAmount+"/"+damageAmountLarge + " damage and regenerate " + healAmount + " health on kill. Stacks within combat infinitely.";
    }

    public override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1, 2, 3};
    }

}

