using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is used to record the full events of a combat:
//specifically where units were spawned, where units fought, and which units survived
public struct CombatEvent {
    Unit source;
    Unit colat;
    HexSquare sourcePos;
    HexSquare colatPos;


    public CombatEvent(Unit source, Unit colat) {
        this.source = source;
        this.colat = colat;
        this.sourcePos = source.getPosition();
        this.colatPos = colat.getPosition();
    }
}

public class CombatLog : MonoBehaviour
{
    Dictionary<Unit, HexSquare> initialPositions;
    List<CombatEvent> log;

    public void initLog(List<Unit> units) {
        this.initialPositions = new Dictionary<Unit, HexSquare>();
        this.log = new List<CombatEvent>();

        foreach(Unit u in units) {
            this.initialPositions.Add(u, u.getPosition());
        }
    }

    public void logEvent(CombatEvent ev) {
        this.log.Add(ev);
    }
}
