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
        Operation buff = new Operation(OperationType.PreMultiply, amt, "Artisan Stack for " + target.getName());
        traitHaver.buffStat(BuffableStats.Damage, buff, traitHaver);
        traitHaver.heal(healAmount);

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();
        cypher.Add(new CharacterTransformation(target.getPosition(), traitHaver.getPosition()));
        cypher.Add(new CharacterTransformation(traitHaver.getPosition(), target.getPosition()));

        this.addToCypher(cypher);
    }

    public override string getTraitName() {
        return "artisans";
    }

    protected override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1, 2, 3};
    }

}

