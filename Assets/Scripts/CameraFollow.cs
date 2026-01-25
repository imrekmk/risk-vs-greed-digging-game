using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, -10f);
    public float smoothTime = 0.08f;

    private Vector3 vel;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = new Vector3(0f, target.position.y, 0f) + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, smoothTime);
    }
}
