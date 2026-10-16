using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DruidTrait : InformationTrait
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private ParticleManager pm;
    [SerializeField] private Player traitPlayer;
    [SerializeField] private float statBoostAmountV;
    [SerializeField] private float statBoostAmountY;
    [SerializeField] private float nonDruidStatBoostAmount;

    private List<HexSquare> vowelSquares;
    private HexSquare ySquare;

    public override void registerUnit(bool addingUnit, Unit u) {
        base.registerUnit(addingUnit, u);

        pm.hideLeaves(this.traitPlayer is HumanPlayer);
        if(this.getMinimumForActive() <= this.getNumberActive()) {
            this.activateDruidSquares();
        }
    }

    private void activateDruidSquares() {
        this.vowelSquares = new List<HexSquare>();
        char[] vowels = {'a', 'e', 'i', 'o', 'u'};

        foreach (HexSquare s in (this.traitPlayer is HumanPlayer) ? this.combatManager.getBoard().getPlayerSquares() : this.combatManager.getBoard().getEnemySquares()) {
            Debug.Log("S CHAR VAL: " + s.getCharValue());
            if (vowels.Contains(s.getCharValue())) {
                this.vowelSquares.Add(s);
            } else if(s.getCharValue() == 'y') {
                this.ySquare = s;
            }
        }

        if(this.vowelSquares.Count == 0) {
            return;
        }

        Debug.Log("VOWELS FOUND: " + this.vowelSquares.Count);

        List<GameObject> leafParticles = this.pm.requestLeaf(this.traitPlayer is HumanPlayer);

        for(int i = 0; i < 5; i++) {
            this.vowelSquares[i].showDruidInfluence(leafParticles[i+1]);
        }

        this.ySquare.showDruidInfluence(leafParticles[0]);
    }

    public override void onCombatStart(Unit traitHaver) {
        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Start));
        if (vowelSquares.Contains(traitHaver.getPosition())) {
            Operation buffOpp = new Operation(OperationType.PostMultiply, this.statBoostAmountV, "Druid Start Buff", true);
            traitHaver.buffStat(BuffableStats.Damage, buffOpp, traitHaver);
            traitHaver.buffStat(BuffableStats.Health, buffOpp, traitHaver);
            return;
        } else if(traitHaver.getPosition() == ySquare) {
            Operation buffOpp = new Operation(OperationType.PostMultiply, this.statBoostAmountY, "Druid Start Buff", true);
            traitHaver.buffStat(BuffableStats.Damage, buffOpp, traitHaver);
            traitHaver.buffStat(BuffableStats.Health, buffOpp, traitHaver);
            return;
        }
    }

    public override void preCombatFinal(Unit traitHaver) {
        foreach (HexSquare s in vowelSquares) {
            if(s.getUnit() != null) {
                //this is a little hacky - it runs up to three times
                //but won't multibuff any unit because Druid Start Buff is always given which means it won't stack
                Operation buffOpp = new Operation(OperationType.PostMultiply, this.nonDruidStatBoostAmount, "Druid Start Buff", true);
                traitHaver.buffStat(BuffableStats.Damage, buffOpp, s.getUnit());
                traitHaver.buffStat(BuffableStats.Health, buffOpp, s.getUnit());
            }
        }
    }

    public override string getTraitName() {
        return "draeioudys";
    }

    public override string getTraitNarrative() {
        return "A gathering of mages in tune with the land.";
    }

    public override string getTraitEffect() {
        return (
            "Spawn 6 ley lines, 5 green and one purple, onto the battlefield, granting " + statBoostAmountV + " times more damage and health from green energy and " + statBoostAmountY + " times more damage and health from purple energy. Non draeioudys only gain " + this.nonDruidStatBoostAmount + " and do not receive an extra purple energy boost."
        );
    }
    

    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }

}
