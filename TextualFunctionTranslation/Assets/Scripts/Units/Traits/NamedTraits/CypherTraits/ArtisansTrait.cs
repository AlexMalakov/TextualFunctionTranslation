using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtisansTrait : CypherTrait
{
    [SerializeField] private float healAmount;
    [SerializeField] private float damageAmountSmall;
    [SerializeField] private float damageAmount;
    [SerializeField] private float damageAmountLarge;

    public virtual void onEnemyDefeat(Unit traitHaver, Unit target){
        int amt = this.getNumberActive() == 1 ? this.damageAmountSmall : (this.getNumberActive() == 2 ? this.damageAmount : this.damageAmountLarge);
        Operation buff = new Operation(OperationType.PreMultiply, amt, "Artisan Stack for " + died.getName());
        traitHaver.buffStat(BuffableStats.Damage, buff);
        //TODO heal unit

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

}

