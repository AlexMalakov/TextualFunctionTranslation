using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum SateliteParticlesActivationType
{
    Spawn,
    FollowUntilCollide,
    AfterShellDestroyed
}

[System.Serializable]
public class SateliteParticles
{
    public ParticleSystem ParticleSystem;
    public SateliteParticlesActivationType ActivationType;
}

[System.Serializable]
public class GunParticlesPair
{
    public Gun Gun;
    public List<SateliteParticles> SateliteParticles;
}

public class SateliteParticlesActivationSystem : MonoBehaviour
{
    [SerializeField] private List<GunParticlesPair> _gunParticlesPairs;
    private void Start()
    {
        foreach (var gun in _gunParticlesPairs.Select(p => p.Gun))
        {
            AddParticlesSupport(gun);
        }
    }

    public void AddParticlesSupport(Gun gun)
    {
        gun.Shot.AddListener(AddParticles);
    }

    public void RemoveParticlesSupport(Gun gun)
    {
        gun.Shot.RemoveListener(AddParticles);
    }

    public void AddParticles(Gun gun, Rigidbody rigidbody)
    {
        if(rigidbody.GetComponent<Shell>())
        {
            var shell = rigidbody.GetComponent<Shell>();
            if(_gunParticlesPairs.Select(e => e.Gun).ToList().Contains(gun))
            {
                var particles = _gunParticlesPairs.Where(e => e.Gun == gun).SingleOrDefault().SateliteParticles;
                foreach(var p in particles)
                {
                    AddParticles(shell, p);
                }
            }
        }
    }

    public void AddParticles(Shell shell, SateliteParticles sateliteParticles)
    {

        switch (sateliteParticles.ActivationType)
        {
            case SateliteParticlesActivationType.Spawn:
                var particles = Instantiate(sateliteParticles.ParticleSystem, shell.transform.position, shell.transform.rotation);
                particles.gameObject.SetActive(true);
                particles.gameObject.AddComponent<TemporaryParticles>();
                break;

            case SateliteParticlesActivationType.FollowUntilCollide:
                var particles1 = Instantiate(sateliteParticles.ParticleSystem, shell.transform.position, Quaternion.identity);
                particles1.gameObject.AddComponent<SelfClearParticles>();
                particles1.transform.parent = shell.transform;
                particles1.transform.localScale = Vector3.one;
                shell.onCollided.AddListener((c) => {
                    if (particles1 == null) return;
                    particles1.transform.parent = null;
                    particles1.transform.localScale = Vector3.one;
                    particles1.Stop(); 
                });
                break;

            case SateliteParticlesActivationType.AfterShellDestroyed: 
                shell.onDestroy.AddListener(
                    () => Instantiate(sateliteParticles.ParticleSystem, shell.transform.position, Quaternion.identity)
                    .gameObject
                    .AddComponent<TemporaryParticles>()
                    ); 
                break;

            default: throw new System.InvalidOperationException("SateliteParticlesActivationType does not choosed. It must be Spawn, FollowUntilCollide or AfterShellDestroyed");
        }
            
    }
}

[RequireComponent(typeof(ParticleSystem))]
public class SelfClearParticles : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!_particleSystem.IsAlive()) Destroy(gameObject);
    }
}
