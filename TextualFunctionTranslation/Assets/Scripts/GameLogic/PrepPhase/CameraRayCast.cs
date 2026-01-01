using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    [SerializeField] private MouseTransform mt;

    public void checkUserInteraction() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {
                    this.onUnitPressed(hit.collider.gameObject.GetComponentInParent<Unit>());
                }
            }
        } else if(Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {
                    this.releaseUnit();
                }
            }
        }
    }


    private void onUnitPressed(Unit u) {
        this.mt.startUnitDrag(u);
    }

    public void releaseUnit() {
        this.mt.endUnitDrag();
    }
}