using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ParticleType {
    Boom, Fart, Star, Heal, Damage, Health, Speed
}

[System.Serializable]
public struct ParticlePool
{
    public GameObject p;
    public List<GameObject> l;
    public int c;

    public void init() {
        for(int i = 0; i < c; i++) {
            l.Add(ParticleManager.Larper.FakeInstantiate(p));
            l[l.Count - 1].SetActive(false);
        }
    }

    public GameObject next() {
        for(int i = 0; i < l.Count; i++) {
            if(!l[i].activeInHierarchy) {
                l[i].SetActive(true);
                return l[i];
            }

        }
        l.Add(ParticleManager.Larper.FakeInstantiate(p));
        return l[l.Count - 1];
    }
}

class UnitParticle {
    public GameObject p;
    public ParticleType pt;
    public float e;

    public UnitParticle(GameObject pp, ParticleType ppt, float exp) {
        p = pp; pt = ppt; e = exp;
    }
}

public class ParticleManager : MonoBehaviour
{
    
    //this should all be in custom serialized structs but htis shit needs to be done in less than 9 days so womp womp to good design
    [SerializeField] private GameObject cam;
    [SerializeField] private MonoLarper serLarper;
    public static MonoLarper Larper;
    [Header ("Leaves")]
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private List<GameObject> playerLeaves;
    [SerializeField] private List<GameObject> enemyLeaves;

    [Header ("Explosions")]
    [SerializeField] private ParticlePool booms;

    [Header ("Fart")]
    [SerializeField] private ParticlePool farts;

    [Header ("Player status particles")]
    [SerializeField] private ParticlePool stars;
    [SerializeField] private ParticlePool healingPs;
    [SerializeField] private ParticlePool damagePs;
    [SerializeField] private ParticlePool healthPs;
    [SerializeField] private ParticlePool speedPs;

    private Dictionary<Unit, List<UnitParticle>> unitParts = new Dictionary<Unit, List<UnitParticle>>();

    [Header ("Projectiles")]
    [SerializeField] private ParticlePool arrows;
    [SerializeField] private ParticlePool magics;
    


    public void Awake() {
        Larper = serLarper;

        for(int i = 0; i < 6; i++) {
            GameObject l1 = Instantiate(this.leafPrefab);
            GameObject l2 = Instantiate(this.leafPrefab);
            l1.SetActive(false);
            l2.SetActive(false);
            this.playerLeaves.Add(l1);
            this.enemyLeaves.Add(l2);
        }

        this.booms.init();
        this.farts.init();
        this.stars.init();
        this.healingPs.init();
        this.damagePs.init();
        this.healthPs.init();
        this.speedPs.init();
        this.arrows.init();
        this.magics.init();
    }
    


    public void requestParticle(HexSquare s, ParticleType pt, float dur) {
        GameObject part = pt == ParticleType.Boom ? this.booms.next() : this.farts.next();
        part.transform.position = new Vector3(s.transform.position.x, part.transform.position.y, s.transform.position.z);
        part.GetComponent<ParticleSystem>().Play();
        StartCoroutine(this.clean(part, dur));
    }
    
    public void pruneFromParticleMap(Unit u) {
        if(this.unitParts.ContainsKey(u)) {
            this.unitParts.Remove(u);
        }
    }

    public void requestPlayerParticle(Unit u, ParticleType pt, float dur) {
        if(this.unitParts.ContainsKey(u)) {
            for(int i = 0; i < this.unitParts[u].Count; i++) {
                UnitParticle up = this.unitParts[u][i];
                if(up.pt == pt) {
                    if(pt == ParticleType.Star) {
                        up.p.GetComponent<FakeStarParticle>().increaseDurationTo(dur);
                    }
                    up.e = Time.time + dur;
                    return;
                }
            }
        } else {
            this.unitParts.Add(u, new List<UnitParticle>());
        }

        GameObject newP;
        switch(pt) {
            case ParticleType.Star:
                newP = this.stars.next();
                newP.GetComponent<FakeStarParticle>().doOrbit(dur, cam, u);
                break;
            case ParticleType.Heal:
                newP = this.healingPs.next();
                newP.GetComponent<ParticleSystem>().Play();
                break;
            case ParticleType.Health:
                newP = this.healthPs.next();
                newP.GetComponent<ParticleSystem>().Play();
                break;
            case ParticleType.Damage:
                newP = this.damagePs.next();
                newP.GetComponent<ParticleSystem>().Play();
                break;
            case ParticleType.Speed:
                newP = this.speedPs.next();
                newP.GetComponent<ParticleSystem>().Play();
                break;
            default:
                return;
        }

        UnitParticle upp = new UnitParticle(newP, pt, Time.time + dur);
        this.unitParts[u].Add(upp);
        newP.transform.position = u.transform.position;

        if(pt != ParticleType.Star) {
            StartCoroutine(this.cleanUpp(u, upp));
        }
    }

    public void cancelUnitParticles(Unit u) {
        if(!this.unitParts.ContainsKey(u))
            return;

        for(int i = 0; i < this.unitParts[u].Count; i++) {
            UnitParticle up = this.unitParts[u][i];
            up.e = 0f;
            if(up.pt == ParticleType.Star) {
                up.p.GetComponent<FakeStarParticle>().cancelAnim();
            } else {
                up.p.SetActive(false);
            }
        }

        this.unitParts[u] = new List<UnitParticle>();
    }

    public void cancelUnitParticle(Unit u, ParticleType pt) {
        if(!this.unitParts.ContainsKey(u))
            return;

        for(int i = 0; i < this.unitParts[u].Count; i++) {
            UnitParticle up = this.unitParts[u][i];
            if(up.pt == pt) {
                up.e = 0f;
                if(up.pt == ParticleType.Star) {
                    up.p.GetComponent<FakeStarParticle>().cancelAnim();
                } else {
                    up.p.SetActive(false);
                }

                this.unitParts[u].Remove(up);
                return;
            }
        }
    }

    public void requestProjectile(Unit attacker, Unit target, int attackType) {
        if(attackType < 1 || attackType > 2) {
            return;
        }

        GameObject projectile = attackType == 1 ? this.arrows.next() : this.magics.next();
        StartCoroutine(projectile.GetComponent<Projectile>().launchProjectile(attacker, target));
    }




    private IEnumerator clean(GameObject launchMe, float duration) {
        yield return new WaitForSeconds(duration);
        launchMe.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        launchMe.SetActive(false);
    }

    private IEnumerator cleanUpp(Unit u, UnitParticle launchMe) {
        while(launchMe.e > Time.time) {
            if(!this.unitParts.ContainsKey(u) || !this.unitParts[u].Contains(launchMe)) yield break;
            yield return null;
        }

        launchMe.p.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        this.unitParts[u].Remove(launchMe);
        launchMe.p.SetActive(false);
    }




    public List<GameObject> requestLeaf(bool isPlayer) {
        List<GameObject> leavesToUse = isPlayer ? this.playerLeaves : this.enemyLeaves;
        foreach(GameObject l in leavesToUse) {
            l.SetActive(true);
            l.GetComponent<LeafParticle>().setY(false);
        }
        leavesToUse[0].GetComponent<LeafParticle>().setY(true);
        return leavesToUse;
    }

    public void hideLeaves(bool isPlayer) {
        foreach(GameObject l in isPlayer ? this.playerLeaves : this.enemyLeaves) {
            l.GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            l.SetActive(false);
        }
    }

}
