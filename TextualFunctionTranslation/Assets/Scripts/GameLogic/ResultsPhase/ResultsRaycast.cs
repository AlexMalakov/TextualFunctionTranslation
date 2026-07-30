using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsRaycast : MonoBehaviour
{
 
    [SerializeField] private CombatLog log;
    [SerializeField] private GameObject tombstonePrefab;

    public void Start() {
        this.tombstonePrefab = Instantiate(tombstonePrefab);
        this.tombstonePrefab.SetActive(false);
    }

    private Coroutine unitMovie;

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

    private void endMovie() {
        //TODO: call hide event
        if(this.unitMovie != null) {
            StopCoroutine(this.unitMovie);
            this.unitMovie = null;
        }
    }

    private void onUnitPressed(Unit u) {
        List<CombatEvent> uEvents = this.log.getUnitsEvents(u);
        //TODO: hide all units
        this.unitMovie = StartCoroutine(this.playUnitMovie(u, uEvents));
    }

    private IEnumerator playUnitMovie(Unit u, List<CombatEvent> events) {
        int pointer = 0;
        while(true) {
            this.showEvent(events[pointer]);
            yield return new WaitForSeconds(GlobalData.MovieFrameDuration);
            pointer = (pointer + 1) % events.Count;
            this.hideEvent(events[pointer]);
        }
    }

    private void showEvent(CombatEvent ev) {
        switch(ev.eventType) {
            case CombatEventType.Buff:
                break;
            case CombatEventType.Kill:
                break;
            case CombatEventType.Attack:
                break;
            case CombatEventType.Death:
                this.tombstonePrefab.SetActive(true);
                this.tombstonePrefab.transform.position = new Vector3(
                        ev.sourcePos.transform.position.x,
                        this.tombstonePrefab.transform.position.y,
                        ev.sourcePos.transform.position.z
                );
                ev.source.gameObject.SetActive(false);
                break;
            case CombatEventType.Survival:
                break;
        }
    }

    private void hideEvent(CombatEvent ev) {
        switch(ev.eventType) {
            case CombatEventType.Buff:
                break;
            case CombatEventType.Kill:
                break;
            case CombatEventType.Attack:
                break;
            case CombatEventType.Death:
                this.tombstonePrefab.SetActive(false);
                break;
            case CombatEventType.Survival:
                break;
        }
    }
}

//this is used to record the full events of a combat:
//specifically where units were spawned, where units fought, and which units survived
// public struct CombatEvent {
//     public Unit source;
//     public Unit colat;
//     public HexSquare sourcePos;
//     public HexSquare colatPos;
//     public CombatEventType eventType;
// }