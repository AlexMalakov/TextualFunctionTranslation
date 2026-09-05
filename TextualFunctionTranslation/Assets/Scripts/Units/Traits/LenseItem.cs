using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LenseItem : MonoBehaviour
{
    
    [SerializeField] private HumanPlayer player;
    [SerializeField] private TutorialStage tut;

    private bool tutorialLockOn;
    private bool tutorialActive;

    private bool lenseToggle = false;

    public void toggleLenses(bool status) {
        this.lenseToggle = status;
    }

    public void checkUserInteraction() {
        if(this.tutorialLockOn) {
            return;
        }
        if(this.lenseToggle && player.getLenses() > 0 && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<HexSquare>() != null) {
                    this.useLense(hit.collider.gameObject.GetComponentInParent<HexSquare>());
                }
            }
        }
    }

    private void useLense(HexSquare s) {
        if(this.tutorialActive && !this.tut.onLenseUse(s)) {
            return;
        }

        if(s.isShowingChar()) {
            return;
        }

        this.player.gainLenses(-1);
        s.displayValue(true);
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
    }

    public void tutorialStarted(bool started) {
        this.tutorialActive = started;
    }
}
