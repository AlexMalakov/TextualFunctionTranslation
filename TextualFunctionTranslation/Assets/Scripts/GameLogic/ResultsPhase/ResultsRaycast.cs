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
        List<CombatEvent> uEvents = this.log.getUnitsEvents(u);
        //TODO: display all events

        foreach(CombatEvent ce in uEvents) {
            switch(ce.eventType) {
                case CombatEventType.Buff:
                    //TODO highlight square of buffed unit, buff effect?
                    HexSquare sq = ce.colatPos;
                    break;
                case CombatEventType.Kill:
                    //TODO highlight square of killed unit, death effect?
                    HexSquare sq = ce.colatPos;
                    break;
                case CombatEventType.Attack:
                    //TODO add particles on damaged unit
                    HexSquare sq = ce.colatPos;
                    break;
                case CombatEventType.Death:
                    //TODO: spawn gravestone
                    HexSquare sq = ce.sourcePos;
                    break;
                case CombatEventType.Survival:
                    //TODO: show unit in that position with nothing else on board
                    HexSquare sq = ce.sourcePos;
                    break;
            }
        }

    }
}
