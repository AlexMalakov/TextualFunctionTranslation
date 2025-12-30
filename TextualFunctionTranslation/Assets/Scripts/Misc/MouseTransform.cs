using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTransform : MonoBehaviour
{
    public float liftHeight = 0.5f;
    private Plane dragPlane;
    private Unit unitDragging;

    void Start() {
        this.dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void startUnitDrag(Unit u) {
        
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
            this.unitDragging.transform.position = this.transform.position;
        }
    }

    void endUnitDrag() {
        this.unitDragging = null;
        // TODO: snapping logic
    }
}
