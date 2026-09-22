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

    private List<HexSquare> vowelSquares;
    private HexSquare ySquare;

    public override void registerUnit(bool addingUnit) {
        base.registerUnit(addingUnit);

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
        if (vowelSquares.Contains(traitHaver.getPosition())) {
            Operation buffOpp = new Operation(OperationType.PreMultiply, this.statBoostAmountV, "Druid Start Buff");
            traitHaver.buffStat(BuffableStats.Damage, buffOpp, traitHaver);
            traitHaver.buffStat(BuffableStats.Health, buffOpp, traitHaver);
            return;
        } else if(traitHaver.getPosition() == ySquare) {
            Operation buffOpp = new Operation(OperationType.PreMultiply, this.statBoostAmountY, "Druid Start Buff");
            traitHaver.buffStat(BuffableStats.Damage, buffOpp, traitHaver);
            traitHaver.buffStat(BuffableStats.Health, buffOpp, traitHaver);
            return;
        }
    }

    public override string getTraitName() {
        return "druid";
    }

    public override string getTraitDescription() {
        return ("A band of magicians in tune with the magic of the land. Spawn in ley lines on squares that give druids "
            + statBoostAmountV + " and " + statBoostAmountV + " times more damage and health when starting combat on them.");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2};
    }

}
