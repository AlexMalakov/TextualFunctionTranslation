using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsRaycast : MonoBehaviour
{
 
    [SerializeField] private CombatLog log;

    public void checkUserInteraction() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {

                    this.onUnitPressed(hit.collider.gameObject.GetComponentInParent<Unit>());
                }
            }
        }
    }

    private void onUnitPressed(Unit u) {
        //display what the unit
    }
}
