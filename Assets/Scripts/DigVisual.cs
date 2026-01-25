using UnityEngine;

public class DigVisual : MonoBehaviour
{
    public GameObject holePrefab;
    public float holeSpawnInterval = 0.06f;
    public float holeZ = 0.9f; // should be in front of ground wall (wall is at z=1)

    private PlayerDigController dig;
    private float t;

    private void Start()
    {
        dig = GetComponent<PlayerDigController>();
    }

    private void Update()
    {
        if (!GameManager.I.isAlive) return;
        if (dig == null || holePrefab == null) return;

        if (!dig.IsHolding()) return;

        t += Time.deltaTime;
        if (t >= holeSpawnInterval)
        {
            t = 0f;

            // Spawn holes on the ground wall (behind the player)
            Vector3 p = transform.position;
            p.z = holeZ;

            Instantiate(holePrefab, p, Quaternion.identity);
        }
    }
}
