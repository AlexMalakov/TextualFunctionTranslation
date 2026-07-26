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

    public static float SquareRevealDuration;
    public static float CypherRevealDuration;
    public static float BetweenRevealDuration;

    [Header ("larper <3")]
    public static MonoLarper Larper;

    [Header ("unit action states")]
    [SerializeField] float idleTime;
    [SerializeField] float walkTime;
    [SerializeField] float turnTime;
    [SerializeField] float swingTime;
    [SerializeField] float windupMinTime;

    [Header ("results anims")]
    [SerializeField] float squareRevealDur;
    [SerializeField] float cypherRevealDur;
    [SerializeField] float betweenRevealDur;


    [SerializeField] MonoLarper ml; //LARP FOR ME

    public void Awake() {
        TimeIdle = idleTime;
        TimeWalk = walkTime;
        TimeTurn = turnTime;
        TimeSwing = swingTime;
        TimeMinWindup = windupMinTime;
        Larper = ml;
        SquareRevealDuration = squareRevealDur;
        CypherRevealDuration = cypherRevealDur;
        BetweenRevealDuration = betweenRevealDur;
    }

}
