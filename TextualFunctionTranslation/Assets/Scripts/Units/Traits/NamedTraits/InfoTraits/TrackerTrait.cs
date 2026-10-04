using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> assistMap; //TODO: stats from multiple trackers
    private int lenseCount;
    [SerializeField] private HumanPlayer player;

    public override void onPrecombatStart() {
        this.lenseCount = 0;
        this.assistMap = new Dictionary<Unit, List<Unit>>();
    }

    public override void onCombatStart(Unit traitHaver) {
        this.assistMap.Add(traitHaver, new List<Unit>());
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(!this.assistMap[traitHaver].Contains(target)) {
            this.assistMap[traitHaver].Add(target);
        }
    }

    public override void informAllOffDeath(Unit death) {
        foreach(Unit u in this.assistMap.Keys) {
            if(this.assistMap[u].Contains(death)) {
                this.lenseCount++;
                this.log.logEvent(new CombatEvent(u, death, CombatEventType.Attack));
                //TODO: give unit it's combat buff???
            }
        }
    }

    public void giveLenses(bool isHuman) {
        if(isHuman) {
            this.player.gainLenses(this.lenseCount);
        } else {
            this.player.gainLenses(-this.lenseCount);
        }
    }

    public override string getTraitName() {
        return "tracker";
    }

    public override string getTraitNarrative() {
        return "Hunters and foresters capable of following prey for miles.";
    }

    public override string getTraitEffect() {
        return (
            "Collects lenses on kills and assists. Lenses can be used to discover glyphs on the battlefield. Opponent's trackers will steal your lenses instead."
        );
    }


    public override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1};
    }
}
