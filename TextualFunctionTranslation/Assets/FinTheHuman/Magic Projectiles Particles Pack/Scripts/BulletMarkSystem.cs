using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GunMarkPair
{
    public Gun Gun;
    public List<GameObject> Marks;
}

public class BulletMarkSystem : MonoBehaviour
{
    [SerializeField] private List<GunMarkPair> _gunMarkPair;

    private void Start()
    {
        foreach(var gun in _gunMarkPair.Select(p => p.Gun))
        {
            gun.Shot.AddListener(AddBulletMark);
        }
    }

    public void AddBulletMark(Gun gun, Rigidbody rigidbody)
    {
        if (rigidbody.GetComponent<Shell>())
        {
            var shell = rigidbody.GetComponent<Shell>();
            if (_gunMarkPair.Select(e => e.Gun).ToList().Contains(gun))
            {
                var marks = _gunMarkPair.Where(e => e.Gun == gun).FirstOrDefault().Marks;
                foreach (var m in marks)
                {
                    shell.onCollided.AddListener((collision) => CreateMark(m, shell.transform.position, Quaternion.LookRotation(collision.contacts[0].normal) * Quaternion.Euler(90,0,0)));
                }
            }
        }
    }

    public GameObject CreateMark(GameObject mark, Vector3 position, Quaternion rotation)
    {
        return Instantiate(mark, position, rotation);
    }
}
