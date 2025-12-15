using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Unit> playersUnits;
    [SerializeField] private PlayerUnitLayout unitLayout;

    public void init() {
        this.unitLayout.initLayout();
    }

    public List<Unit> getActiveUnits() {
        return this.playersUnits;
    }

    public PlayerUnitLayout getUnitLayout() {
        return this.unitLayout;
    }
}
