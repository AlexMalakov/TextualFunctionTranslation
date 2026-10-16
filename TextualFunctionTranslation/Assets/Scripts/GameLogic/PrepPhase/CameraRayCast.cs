using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField] private MouseTransform mt;

    private bool toggle = false;
    private bool tutorialLockOn = false;
    private bool diaLockIsOn = false;
    private bool popUpLockOn = false;

    public void checkUserInteraction() {
        if(this.tutorialLockOn || this.diaLockIsOn || this.popUpLockOn) {
            if(toggle) {
                this.releaseUnit();
                toggle = false;
            }

            return; //no interacting while 
        }

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {
                    toggle = true;
                    this.onUnitPressed(hit.collider.gameObject.GetComponentInParent<Unit>());
                }
            }
        } else if(Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (toggle) {
                    this.releaseUnit();
                    toggle = false;
                }
            }
        }
    }

    public void tutorialLock(bool lockActive) {
        this.tutorialLockOn = lockActive;
        if(toggle) {
            this.releaseUnit();
            toggle = false;
        }
    }

    public void diaLock(bool lockActive) {
        this.diaLockIsOn = lockActive;
        if(toggle) {
            this.releaseUnit();
            toggle = false;
        }
    }

    public void popUpLock(bool lockActive) {
        this.popUpLockOn = lockActive;
        if(toggle) {
            this.releaseUnit();
            toggle = false;
        }
    }

    public void tutorialStarted(bool started) {
        this.mt.tutorialStarted(started);
    }

    private void onUnitPressed(Unit u) {
        this.mt.startUnitDrag(u);
    }

    public void releaseUnit() {
        this.mt.endUnitDrag();
    }
}