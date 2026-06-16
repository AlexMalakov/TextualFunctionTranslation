using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CombatEventType {
    Buff,
    Attack,
    Kill,
    Death,
    Survival
}

//this is used to record the full events of a combat:
//specifically where units were spawned, where units fought, and which units survived
public struct CombatEvent {
    public Unit source;
    public Unit colat;
    public HexSquare sourcePos;
    public HexSquare colatPos;
    public CombatEventType eventType;


    public CombatEvent(Unit source, Unit colat, CombatEventType eventType) {
        this.source = source;
        this.colat = colat;
        this.sourcePos = source.getPosition();
        this.colatPos = colat.getPosition();
        this.eventType = eventType;
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

    public List<CombatEvent> getUnitsEvents(Unit u) {
        List<CombatEvent> uLog = new List<CombatEvent>();

        foreach(CombatEvent e in this.log) {
            if(e.source == u) {
                uLog.Add(e);
            }
        }

        return uLog;
    }
}
