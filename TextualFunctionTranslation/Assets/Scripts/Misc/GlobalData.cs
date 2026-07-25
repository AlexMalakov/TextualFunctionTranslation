using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static float TimeIdle;
    public static float TimeWalk;
    public static float TimeTurn;
    public static float TimeSwing;
    public static float TimeMinWindup;

    public static MonoLarper Larper;

    [SerializeField] float idleTime;
    [SerializeField] float walkTime;
    [SerializeField] float turnTime;
    [SerializeField] float swingTime;
    [SerializeField] float windupMinTime;

    [SerializeField] MonoLarper ml; //LARP FOR ME

    public void Awake() {
        TimeIdle = idleTime;
        TimeWalk = walkTime;
        TimeTurn = turnTime;
        TimeSwing = swingTime;
        TimeMinWindup = windupMinTime;
        Larper = ml;
    }

}
