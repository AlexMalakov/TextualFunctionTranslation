using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScheduler : MonoBehaviour
{
    [SerializeField] public float combatTickTime = .05f; //according to chat this is a good refresh rate
    [SerializeField] public CombatManager manager;

    public static float combatTimePerTick;

    private float lastCheckIn;

    public void Awake() { //Awake because this doesn't affect other systems
        combatTimePerTick = this.combatTickTime;
    }

    public void onCombatStart() {
        this.lastCheckIn = Time.time;
    }

    public void runNUpdates() {
        float newCheckIn = Time.time;

        if((newCheckIn - this.lastCheckIn)/combatTickTime < 1) {
            return;
        }

        int n = (int)((newCheckIn - this.lastCheckIn)/combatTickTime);
        for(int i = 1; i <= n; i++) {
            //call update?
            manager.runAttackFrame();
        }

        lastCheckIn += combatTickTime * n;
    }
}
