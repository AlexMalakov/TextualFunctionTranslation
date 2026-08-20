using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    [SerializeField] protected List<Unit> playersUnits;
    [SerializeField] protected PlayerUnitLayout unitLayout;
    [SerializeField] private Quaternion unitRotation;
    [SerializeField] private List<Trait> allTraits;

    public virtual void init() {
        this.unitLayout.initLayout();
    }

    public void assignLayout(PlayerUnitLayout layout) {
        this.unitLayout = layout;
    }

    public List<Unit> getActiveUnits() {
        return this.unitLayout.getActiveUnits();
    }

    public virtual PlayerUnitLayout getUnitLayout() {
        return this.unitLayout;
    }

    public bool ownsUnit(Unit u) {
        return this.playersUnits.Contains(u);
    }

    //returns true if a unit is in the combat layout
    public bool unitIsActive(Unit u) {
        return this.unitLayout.getActiveUnits().Contains(u);
    }

    public Quaternion getUnitRotation(bool bench) {
        return bench? this.unitRotation * Quaternion.Euler(0f, 180f, 0f) : this.unitRotation;
    }

    public List<Trait> getAllTraits() {
        return this.allTraits;
    }
}
