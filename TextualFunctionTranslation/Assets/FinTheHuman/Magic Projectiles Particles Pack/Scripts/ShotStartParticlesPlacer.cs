using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotStartParticlesPlacer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _shotStartParticles;

    public void ActivateParticles()
    {
        foreach (var e in _shotStartParticles)
        {
            var particles = Instantiate<ParticleSystem>(e, transform.position, transform.rotation);
            particles.gameObject.SetActive(true);
        }
    }
}
