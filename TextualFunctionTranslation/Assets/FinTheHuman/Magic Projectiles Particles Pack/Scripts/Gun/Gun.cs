using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] private float _maxRayLenght = 1000;
    [SerializeField] private Rigidbody _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _projectileSpeed = 100;

    public UnityEvent<Gun, Rigidbody> Shot;
    public Vector3 Target;
    public Transform FirePoint => _firePoint;
    public float MaxRayLenght => _maxRayLenght;
    
    public void FireProjectile()
    {
        var projectile = Instantiate(_projectile, _firePoint.position, Quaternion.identity);
        projectile.transform.LookAt(Target);
        projectile.gameObject.SetActive(true);
        projectile.AddForce((Target - transform.position).normalized * _projectileSpeed);

        Shot.Invoke(this, projectile);
    }
}
