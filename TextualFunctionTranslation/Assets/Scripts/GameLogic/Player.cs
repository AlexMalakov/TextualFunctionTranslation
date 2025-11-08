using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Unit> playersUnits;


    public List<Unit> getUnits() {
        return this.playersUnits;
    }
}
