using UnityEngine;

public class StuckingShell : Shell
{
    private void Start()
    {
        onCollided.AddListener(c => Stuck(c));
    }

    private void Stuck(Collision collision)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
