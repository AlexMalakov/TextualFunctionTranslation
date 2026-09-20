using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitSellDock : MonoBehaviour
{
    [SerializeField] private RectTransform sellArea;
    [SerializeField] private HumanPlayer player;

    [SerializeField] private TMP_Text sellCost;
    [SerializeField] private GameObject grayCover;

    private Unit dragging;
    private bool tutorialLockOn;

    public void isUnitBeingDragged(bool dragged, Unit u) {
        this.dragging = u;

        if(u != null) {
            this.sellCost.text = ""+(u.getCost() - 1);
        } else {
            this.sellCost.text = "0";
        }

        this.gameObject.SetActive(dragged);
    }

    public bool isGoingToSellUnit() {
        if (!this.tutorialLockOn && Input.GetMouseButtonUp(0) && this.dragging != null) {
            if (RectTransformUtility.RectangleContainsScreenPoint(this.sellArea, Input.mousePosition)) {
                player.sellUnit(this.dragging);
                this.dragging = null;
                return true;
            }
        }
        return false;
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
        this.grayCover.SetActive(lockOn);
    }
}
