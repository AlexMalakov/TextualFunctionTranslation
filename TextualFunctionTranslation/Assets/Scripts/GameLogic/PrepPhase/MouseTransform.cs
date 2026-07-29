using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTransform : MonoBehaviour
{
    [SerializeField] private float snapRadius = 1f;
    [SerializeField] private Player humanPlayer;
    private Plane dragPlane;
    private Unit unitDragging;
    private Vector3 initialPos;

    void Awake() { //Awake because this doesn't affect other systems
        this.dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void startUnitDrag(Unit u) {
        if(!this.humanPlayer.ownsUnit(u)) {
            return;
        }

        u.triggerAnimation("falling");
        this.humanPlayer.getUnitMoveManager().overrideMovement(u);

        this.initialPos = u.transform.position;
        this.unitDragging = u;
    }

    void Update() {
        if(this.unitDragging == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter)) {
            Vector3 point = ray.GetPoint(enter);
            point.y = this.initialPos.y;
            this.transform.position = point;
            this.unitDragging.transform.position = point;
        }
    }

    public void endUnitDrag() {
        bool succeeded = false;

        //snapping logicL
        if(this.unitDragging != null) {
            Collider[] hits = Physics.OverlapSphere(this.transform.position, this.snapRadius);
            this.unitDragging.triggerAnimation("landing");

            float closestObj = float.MaxValue;
            BenchSlot closestBench = null;
            HexSquare closestSquare = null;
            bool square = true;

            foreach(Collider c in hits) {
                //debug here might need different get component stuff
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

            
            //call update and see if it succeeded or not
            if(square && closestSquare != null) {
                succeeded = this.humanPlayer.getUnitLayout().attemptUpdateLayout(this.unitDragging, closestSquare);
            } else if(!square && closestBench != null) {
                succeeded = this.humanPlayer.getBench().updateBenchLayout(this.unitDragging, closestBench.getSlotNum());
            }
        } else {
            succeeded = true;
        }


        if(!succeeded) {
            StartCoroutine(this.humanPlayer.getUnitMoveManager().dragUnitTo(this.unitDragging, this.initialPos, false));
        }
        this.unitDragging = null;
    }
}
