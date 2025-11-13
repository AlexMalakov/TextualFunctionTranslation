using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScheduler : MonoBehaviour
{
    [SerializeField] public float combatTickTime = .05f; //according to chat this is a good refresh rate
    [SerializeField] public CombatManager manager;

    private float lastCheckIn;



    public void runNUpdates() {
        float newCheckIn = Time.time;

        for(int i = 1; i <= (newCheckIn - this.lastCheckIn)/combatTickTime; i++) {
            //call update?
            manager.runAttackFrame();
        }

        lastCheckIn = newCheckIn;
    }
}
