using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCheck : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private HumanPlayer player;
    [SerializeField] private Player enemy;

    //TODO: wire up

    private bool failed = false;
    private bool done = false;

    public void precombatCheck() {
        this.failed = false; //reset
        this.done = false;

        if(!this.checkKing()) {
            this.failed = true;
            return;
        }

        if(!this.outcastsAlone()) {
            this.failed = true;
            return;
        }

        if(!this.noDups()) {
            this.failed = true;
            return;
        }

        if(!this.circleJerk()) {
            this.failed = true;
            return;
        }

        if(!this.isBroke()) {
            this.failed = true;
            return;
        }

        //haven't failed yet, just need death unit to die now
    }

    private bool checkKing() {
        bool hasKing = false;
        foreach(Unit u in this.enemy.getActiveUnits()) {
            hasKing = hasKing || (u is DemonKing);
        }

        return hasKing;
    }

    private bool outcastsAlone() {
        List<Unit> outcasts = new List<Unit>();
        foreach(Unit u in this.player.getActiveUnits()) {
            if(u.getRoleTrait().getTraitName() == "outcast") {
                outcasts.Add(u);
            }
        }

        if(outcasts.Count < 2) {
            return false;
        }

        bool selfBuffing = false;
        foreach(Unit o in outcasts) {
            //todo: check if an outcast has a self buff
        }

        return !selfBuffing;
    }

    private bool noDups() { //TODO: lets check active units isnt returning bench
        List<string> unitNames = new List<string>();
        foreach(Unit u in this.player.getActiveUnits()) {
            if(unitName.Contains(u.getUnitName())) {
                return false;
            }
        }
        return true;
    }

    private bool circleJerk() {
        foreach(Unit u in this.player.getActiveUnits()) {
            //TODO: check if a non self buff is present
        }
        return true;
    }

    private bool isBroke() {
        return this.player.getMoney() == 0;
    }

    public void onDeathCheck(Unit dead) {
        if(!this.failed) {
            if(this.isSacrifice(dead)) {
                this.shop.revealHero();
            }
            
            this.failed = true; //even when hero is revealed so that we don't waste effort once we're done
        }
    }

    private bool isSacrifice(Unit dead) {
        return dead.getRoleTrait().getTraitName() == "goblin" || dead.getRoleTrait().getTraitName() == "plague doctor"
    }

}
