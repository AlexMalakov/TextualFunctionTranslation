using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Unit> playersUnits;
    [SerializeField] private PlayerUnitLayout unitLayout;
    [SerializeField] private Bench bench;
    [SerializeField] private UnitMovementManager umm;

    [SerializeField] private int unitCap; //TODO: upadte this based on the current round
    

    public void init() {
        this.unitLayout.initLayout();
    }

    public List<Unit> getActiveUnits() {
        return this.unitLayout.getActiveUnits();
    }

    public PlayerUnitLayout getUnitLayout() {
        return this.unitLayout;
    }

    public Bench getBench() {
        return this.bench;
    }

    public UnitMovementManager getUnitMoveManager() {
        return this.umm;
    }

    public bool ownsUnit(Unit u) {
        return this.playersUnits.Contains(u);
    }

    //returns true if a unit is in the combat layout
    public bool unitIsActive(Unit u) {
        return this.unitLayout.getActiveUnits().Contains(u);
    }

    public int getMaxCombatUnits() {
        return this.unitCap;
    }
}
