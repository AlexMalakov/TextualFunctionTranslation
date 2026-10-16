using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private ParticleManager pm;

    [SerializeField] private float damageAmountSmall;
    [SerializeField] private float damageAmount;
    [SerializeField] private float damageAmountLarge;

    [SerializeField] private float healPercentSmall;
    [SerializeField] private float healPercent;
    [SerializeField] private float healPercentLarge;
    
    public override void onUnitDeath(Unit traitHaver, Unit killer) {

        this.log.logEvent(new CombatEvent(traitHaver, killer, CombatEventType.Death));
        
        List<HexSquare> radCircle = this.getSquaresInRadius(traitHaver.getPosition(), 1, this.cm.getBoard());

        foreach(HexSquare s in radCircle) {
            this.pm.requestParticle(s, ParticleType.Boom, 1f);
        }

        float damage = 0;
        float heal = 0;

        if(this.getNumberActive() == 1) {
            damage = this.damageAmountSmall;
            heal = this.healPercentSmall;
        } else if(this.getNumberActive() == 2) {
            damage = this.damageAmount;
            heal = this.healPercent;
        } else if(this.getNumberActive() >= 3) {
            damage = this.damageAmountLarge;
            heal = this.healPercentLarge;
        }

        foreach(HexSquare s in radCircle) {
            if(s.getUnit() != null && this.cm.sameOwner(s.getUnit(), traitHaver)) {
                s.getUnit().heal(s.getUnit().calcMaxHealth() * heal/100f);
            } else if(s.getUnit() != null) {
                s.getUnit().takeDamage(damage, traitHaver);
            }
        }

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        HexSquare prev = null;
        foreach(HexSquare s in radCircle) {
            if(prev != null) {
                cypher.Add(new CharacterTransformation(s, prev));
            }

            prev = s;
        }
        cypher.Add(new CharacterTransformation(radCircle[0], prev));

        this.addToCypher(cypher);
    }

    public override string getTraitName() {
        return "troll";
    }

    public override string getTraitNarrative() {
        return "A local band of trolls gifted with club and proficient with explosives.";
    }

    public override string getTraitEffect() {
        return (
            "On death, explodes. Explosion targets surrounding squares, dealing " + this.damageAmountSmall+"/"+this.damageAmount+"/"+damageAmountLarge
            + " damage to enemies, and healing allies for "+ this.healPercentSmall+"/"+this.healPercent+"/"+this.healPercentLarge + " percent of max hp."
        );
    }

    public override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1,2,3};
    }
}