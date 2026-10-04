using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObject : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 2f;
    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}
