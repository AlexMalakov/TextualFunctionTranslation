using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementManager : MonoBehaviour
{
    private List<Unit> draggedUnits = new List<Unit>();
    [SerializeField] private float unitDragSnapSpeed;
    
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

        float elapsed = 0f;
        float totalDur = !snap ? .3f : (dest - startPos).magnitude / this.unitDragSnapSpeed;

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
}
