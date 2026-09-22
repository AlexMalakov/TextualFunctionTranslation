using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafParticle : MonoBehaviour
{
    [SerializeField] private GameObject purp;
    [SerializeField] private GameObject green;

    public void setY(bool isY) {
        this.purp.SetActive(isY);
        this.green.SetActive(!isY);
    }
}
