using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ok so this is a hack that lets me use invoke/coroutine in classes that don't extend monobehavior
//it's existance is unennicsary but will make code cleaner

//how to use:
//for invoke:
//get a MonoLarper object into your script; i dont care how you do it
//<MonoLarper obj>.FakeInvoke(new InvokeableMethod(<method>, <delay>), delay);
//method will run in <delay> seconds (enjoy)

//for coroutine:
//get a MonoLarper object into your script; i dont care how you do it
//<MonoLarper obj>.FakeCoroutine(new CoroutineableMethod(<method>));
//method will run as a coroutine doing all the funny coroutine things (enjoy)
//note: method must return IEnumerator to use as coroutine

public class MonoLarper : MonoBehaviour
{
    public List<InvokeableMethod> invokeEvents = new List<InvokeableMethod>(); //queue of events ordered by start time

    public void FakeInvoke(InvokeableMethod invokeMe) {
        for(int i = 0; i <= invokeEvents.Count; i++) { //might not be threadsafe
            if(i == invokeEvents.Count || invokeEvents[i].finishTime > invokeMe.finishTime) {
                invokeEvents.Insert(i, invokeMe);
                break;
            }
        }
        Invoke("InvokeMe", invokeMe.finishTime - Time.time);
    }

    //runs method that is expected to finish first however since this method is only called with invoke, it should
    //always run the method that is expected to run immediately || might not be threadsafe
    private void InvokeMe() {
        invokeEvents[0].execute();
        invokeEvents.RemoveAt(0);
    }

    public Coroutine FakeStartCoroutine(CoroutineableMethod coroutineMe) {
        return StartCoroutine(coroutineMe.execute());
    }
    
    public void FakeStopCoroutine(Coroutine uncoroutineMe) {
        StopCoroutine(uncoroutineMe);
    }

    public GameObject FakeInstantiate(GameObject obj, Vector3 position, Quaternion quaternion) {
        return Instantiate(obj, position, quaternion);
    }

    public void Destroy(GameObject obj) {
        Destroy(obj);
    }
}

public class InvokeableMethod {
    public delegate void Invokeable();
    public float finishTime;
    public event Invokeable invokeable;

    public InvokeableMethod(Invokeable method, float delay) {
        this.finishTime = Time.time + delay;
        invokeable += method;
    }

    public void execute() {
        invokeable();
    }
}

public class CoroutineableMethod {
    public delegate IEnumerator Coroutineable();
    public event Coroutineable coroutineable;

    public CoroutineableMethod(Coroutineable method) {
        coroutineable += method;
    }

    public IEnumerator execute() {
        return coroutineable();
    }
}