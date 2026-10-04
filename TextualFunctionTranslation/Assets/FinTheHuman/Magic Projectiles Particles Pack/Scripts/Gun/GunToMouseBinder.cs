using UnityEngine;

[RequireComponent(typeof(Gun))]
public class GunToMouseBinder : MonoBehaviour
{
    [SerializeField] private KeyCode _trigger = KeyCode.Mouse0;
    private Gun _gun;

    void Start()
    {
        _gun = GetComponent<Gun>();
    }

    void Update()
    {
        _gun.Target = getTargetPosition();
        _gun.transform.LookAt(_gun.Target);

        if (Input.GetKeyDown(_trigger))
            _gun.FireProjectile();
    }

    private Vector3 getTargetPosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return ray.direction * _gun.MaxRayLenght;
        }
    }
}
