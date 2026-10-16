using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerTrait : InformationTrait
{
    private Dictionary<Unit, List<Unit>> assistMap;
    private int lenseCount;
    [SerializeField] private HumanPlayer player;
    [SerializeField] private Player traitsOpponent;
    [SerializeField] private HexBoard board;
    private List<HexSquare> trackingSquares;
    private List<Unit> trackers = new List<Unit>();
    private int buffCounter;

    public override void onPrecombatStart() {
        this.lenseCount = 0;
        this.assistMap = new Dictionary<Unit, List<Unit>>();
        this.trackers = new List<Unit>();
        this.buffCounter = 0;
        
        this.trackingSquares = new List<HexSquare>(this.playersTrait ? this.board.getEnemySquares() : this.board.getPlayerSquares());
        foreach(Unit u in this.traitsOpponent.getActiveUnits()) {
            this.trackingSquares.Remove(u.getPosition());
        }
    }

    public override void informAllOfReposition(Unit mover){
        //any square that's stepped on isn't tracked. Regardless of who mover belongs to
        int before = this.trackingSquares.Count;
        this.trackingSquares.Remove(mover.getPosition());
        int after = this.trackingSquares.Count;

        Operation dmg = new Operation(OperationType.PostAdd, this.getNumberActive(), "Tracker Damage " + buffCounter, false); //time.time to get buff stacking
        if(after < before) {
            foreach(Unit t in this.trackers) {
                if(t.isDead())
                    continue;
                t.buffStat(BuffableStats.Damage, dmg, mover);
            }
        }
        this.buffCounter++;
    }

    public override void onCombatEnd() {
        foreach(HexSquare sq in this.trackingSquares) {
            this.logSquare(sq);
        }
    }

    public override void onCombatStart(Unit traitHaver) {
        this.assistMap.Add(traitHaver, new List<Unit>());
        this.trackers.Add(traitHaver);
    }

    public override void onUnitAttack(Unit traitHaver, Unit target) {
        if(!this.assistMap[traitHaver].Contains(target)) {
            this.assistMap[traitHaver].Add(target);
        }
    }

    public override void informAllOffDeath(Unit death) {
        foreach(Unit u in this.assistMap.Keys) {
            if(u.isDead()) {
                continue;
            }
            if(this.assistMap[u].Contains(death)) {
                this.lenseCount++;
                this.log.logEvent(new CombatEvent(u, death, CombatEventType.Attack));
                foreach(Unit t in this.trackers) {
                    if(t.isDead())
                        continue;
                    t.heal(100f);
                }
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
            "Collects lenses and heals for 100hp while alive on kills and assists. Lenses can be used to discover glyphs on the battlefield. Opponent's trackers will steal your lenses instead. Also gains 1/2/3/4 damage every time a unit steps on an unexplored enemy square."
        );
    }


    public override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1};
    }
}
