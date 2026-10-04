using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class DisolveObject : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 2f;
    private Material _material;

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
        StartCoroutine(nameof(Disolve));
    }

    private IEnumerator Disolve()
    {
        if (_material.color.a > 0)
        {
            while (_lifeTime > 0)
            {
                float c = _material.color.a / _lifeTime * Time.deltaTime;
                _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, _material.color.a - c);
                _lifeTime -= Time.deltaTime;
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
