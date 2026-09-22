using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementManager : MonoBehaviour
{
    private List<Unit> draggedUnits = new List<Unit>();
    [SerializeField] private float unitDragSnapSpeed;
    [SerializeField] private float unitJumpSpeed;
    [SerializeField] private float boingMultiplicitiveHeight;
    [SerializeField] private float unitFastTime = .1f;
    
    public void overrideMovement(Unit u) {
        this.draggedUnits.Remove(u);
    }

    public IEnumerator dragUnitTo(Unit u, Transform dest, bool snap) {
        return this.dragUnitTo(u, dest.position, snap);
    }
    
    public IEnumerator dragUnitTo(Unit u, Vector3 dest, bool snap) {
        if(this.draggedUnits.Contains(u)) { //is this possible? {
            yield break;
        }
        this.draggedUnits.Add(u);

        Vector3 startPos = u.transform.position;

        dest.y = startPos.y;

        float elapsed = 0f;
        float totalDur = !snap ? unitFastTime : (dest - startPos).magnitude / this.unitDragSnapSpeed;

        while(elapsed < totalDur) {
            
            if (!this.draggedUnits.Contains(u)) {
                yield break;
            }

            u.transform.position = Vector3.Lerp(startPos, dest, elapsed/totalDur);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.draggedUnits.Remove(u);
    }

    public IEnumerator boingTo(Unit u, Vector3 dest) {
        if(this.draggedUnits.Contains(u)) { //no boing
            yield break;
        }

        this.draggedUnits.Add(u);
        Vector3 startPos = u.transform.position;
        dest.y = startPos.y;
        u.triggerAnimation("leap");

        float elapsed = 0f;
        float totalDur = (dest - startPos).magnitude / this.unitJumpSpeed;
        float arcHeight = (dest - startPos).magnitude * this.boingMultiplicitiveHeight;

        while(elapsed < totalDur) {
            if(!this.draggedUnits.Contains(u)) {
                yield break;
            }

            Vector3 uPos = Vector3.Lerp(startPos, dest, elapsed/totalDur);
            uPos.y = Mathf.Sin((elapsed/totalDur) * Mathf.PI) * arcHeight;
            u.transform.position = uPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        u.triggerAnimation("landing");
        this.draggedUnits.Remove(u);
    }
}
