using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> assistMap;
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

    public override string getTraitDescription() { //TODO: buff?
        return ("The kingdoms hunters, capable of following any prey for miles. Collect lenses and ___ on takedowns, "
            + ". Can use lenses to uncover hidden energies on hexes of the battle field. Opponent's trackers will instead "
            + "steal your lenses.");
    }

    protected override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1};
    }
}
