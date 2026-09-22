using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [Header ("Leaves")]
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private int leafCount = 12;
    private List<GameObject> leafsPlayer = new List<GameObject>();
    private List<GameObject> leafsEnemy = new List<GameObject>();

    [Header ("Stars")]
    [SerializeField] private GameObject starsPrefab;
    [SerializeField] private int starCount = 10;
    private List<GameObject> stars = new List<GameObject>();
    private Dictionary<Unit, FakeStarParticle> unitToStar;

    [Header ("Explosions")]
    [SerializeField] private GameObject boomPrefab;
    [SerializeField] private int boomCount = 20;
    private List<GameObject> booms = new List<GameObject>();

    [Header ("Fart")]
    [SerializeField] private GameObject fartPrefab;
    [SerializeField] private int fartCount = 20;
    private List<GameObject> farts = new List<GameObject>();



    public void Awake() {
        for(int i = 0; i < this.leafCount/2; i++) {
            this.leafsPlayer.Add(Instantiate(this.leafPrefab));
            this.leafsPlayer[this.leafsPlayer.Count - 1].SetActive(false);
        }

        for(int i = 0; i < this.leafCount/2; i++) {
            this.leafsEnemy.Add(Instantiate(this.leafPrefab));
            this.leafsEnemy[this.leafsEnemy.Count - 1].SetActive(false);
        }

        for(int i = 0; i < this.starCount; i++) {
            this.stars.Add(Instantiate(this.starsPrefab));
            this.stars[this.stars.Count - 1].SetActive(false);
        }

        this.unitToStar = new Dictionary<Unit, FakeStarParticle>();

        for(int i = 0; i < this.boomCount; i++) {
            this.booms.Add(Instantiate(this.boomPrefab));
            this.booms[this.booms.Count - 1].SetActive(false);
        }

        for(int i = 0; i < this.fartCount; i++) {
            this.farts.Add(Instantiate(this.fartPrefab));
            this.farts[this.farts.Count - 1].SetActive(false);
        }
    }

    private int nextInPool(List<GameObject> pool) {
        for(int i = 0; i < pool.Count; i++) {
            if(!pool[i].activeInHierarchy) {
                return i;
            }
        }
        return -1;
    }
    
    //y = 0
    public List<GameObject> requestLeaf(bool isPlayer) {
        List<GameObject> leavesToUse = isPlayer ? this.leafsPlayer : this.leafsEnemy;
        foreach(GameObject l in leavesToUse) {
            l.SetActive(true);
            l.GetComponent<LeafParticle>().setY(false);
        }
        leavesToUse[0].GetComponent<LeafParticle>().setY(true);
        return leavesToUse;
    }

    public void hideLeaves(bool isPlayer) {
        foreach(GameObject l in isPlayer ? this.leafsPlayer : this.leafsEnemy) {
            l.SetActive(false);
        }
    }

    public void requestStar(Unit u, float stunDuration) {
        if(this.unitToStar.ContainsKey(u)) {
            this.unitToStar[u].increaseDurationTo(stunDuration);
            return;
        }

        int index = this.nextInPool(this.stars);
        GameObject nextStar;
        if(index < 0) {
            nextStar = Instantiate(this.starsPrefab);
            this.stars.Add(nextStar); 
        } else {
            nextStar = this.stars[index];
        }

        nextStar.SetActive(true);
        this.unitToStar.Add(u, nextStar.GetComponent<FakeStarParticle>());
        nextStar.GetComponent<FakeStarParticle>().doOrbit(stunDuration, cam, u);
        nextStar.transform.position = u.transform.position;
    }

    public void unassignStar(Unit u) {
        if(this.unitToStar.ContainsKey(u)) {
            this.unitToStar[u].cancelAnim();
            this.unitToStar.Remove(u);
        }
    }

    public void requestBoom(HexSquare s) {
        int index = this.nextInPool(this.booms);
        GameObject nextBoom;
        if(index < 0) {
            nextBoom = Instantiate(this.boomPrefab);
            this.booms.Add(nextBoom); 
        } else {
            nextBoom = this.booms[index];
        }
        nextBoom.SetActive(true);
        nextBoom.transform.position = s.transform.position;
    }

    public void requestFart(HexSquare s) {
        int index = this.nextInPool(this.farts);
        GameObject nextFart;
        if(index < 0) {
            nextFart = Instantiate(this.fartPrefab);
            this.farts.Add(nextFart); 
        } else {
            nextFart = this.farts[index];
        }
        nextFart.SetActive(true);
        nextFart.transform.position = s.transform.position;
    }
}
