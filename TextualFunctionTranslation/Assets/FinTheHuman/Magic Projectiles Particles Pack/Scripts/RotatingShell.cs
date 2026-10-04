using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingShell : MonoBehaviour
{
    [SerializeField] private float _speed;
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, _speed);
    }
}