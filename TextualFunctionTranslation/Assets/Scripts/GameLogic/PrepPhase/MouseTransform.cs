using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTransform : MonoBehaviour
{
    [SerializeField] private float liftHeight = 0.5f;
    private Plane dragPlane;
    private Unit unitDragging;
    private Vector3 initialPos;

    void Start() {
        this.dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void startUnitDrag(Unit u) {
        this.initialPos = u.transform.position;
        this.unitDragging = u;
    }

    void Update() {
        if(this.unitDragging == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter)) {
            Vector3 point = ray.GetPoint(enter);
            point.y += liftHeight;
            this.transform.position = point;
            this.unitDragging.transform.position = point;
        }
    }

    void endUnitDrag() {
        if(this.unitDragging != null) {
            // TODO: snapping logic
            Collider[] hits = Physics.OverlapSphere(this.transform.position, this.snapRadius);

            float closestObj = float.MaxValue;
            Bench closestBench = null;
            HexSquare closestSquare = null;
            bool square = true;

            // foreach(Collider c in hits) {
            //     if(c.gameObject.GetComponentInParent<HexSquare>()) {

            //     } else if(c.gameObject.GetComponentInParent<Bench>()) {

            //     }
            // }


            //call bench or playerLayout => update method
            //if true then do the move, otherwise move unit back to its initialPosition
            //or if no object is found in overlap then also just move it back to initialPosition
        }

        this.unitDragging = null;
    }
}
