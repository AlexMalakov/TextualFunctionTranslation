using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTransform : MonoBehaviour
{
    [SerializeField] private float liftHeight = 0.5f;
    [SerializeField] private Player humanPlayer;
    private Plane dragPlane;
    private Unit unitDragging;
    private Vector3 initialPos;

    void Start() {
        this.dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void startUnitDrag(Unit u) {
        this.humanPlayer.getUnitLayout().unitClickedOn(u);
        this.humanPlayer.getBench().unitClickedOn(u);

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
        bool succeeded = false;
        if(this.unitDragging != null) {
            // TODO: snapping logic
            Collider[] hits = Physics.OverlapSphere(this.transform.position, this.snapRadius);

            float closestObj = float.MaxValue;
            BenchSlot closestBench = null;
            HexSquare closestSquare = null;
            bool square = true;

            foreach(Collider c in hits) {
                if(c.gameObject.GetComponentInParent<HexSquare>() != null) {
                    if((c.gameObject.transform.position - this.transform.position).magnitude < closestObj) {
                        closestObj = (c.gameObject.transform.position - this.transform.position).magnitude;
                        square = true;
                        closestSquare = c.gameObject.GetComponentInParent<HexSquare>();
                    }
                } else if(c.gameObject.GetComponent<BenchSlot>() != null) {
                    if((c.gameObject.transform.position - this.transform.position).magnitude < closestObj) {
                        closestObj = (c.gameObject.transform.position - this.transform.position).magnitude;
                        square = false;
                        closestBench = c.gameObject.GetComponentInParent<BenchSlot>();
                    }
                }
            }

            
            if(square && closestSquare != null) {
                succeeded = this.humanPlayer.getUnitLayout().attemptUpdateLayout(this.unitDragging, closestSquare)
            } else if(!square && closestBench != null) {
                succeeded = this.humanPlayer.getBench().updateBenchLayout(this.unitDragging, closestBench.getSlotNum());
            }


            //call bench or playerLayout => update method
            //if true then do the move, otherwise move unit back to its initialPosition
            //or if no object is found in overlap then also just move it back to initialPosition
        }

        if(!succeeded) {
            StartCoroutine(returnUnit())
        }
        this.unitDragging = null;
    }
}
