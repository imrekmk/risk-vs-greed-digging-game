using UnityEngine;

public class DigDustBurst : MonoBehaviour
{
    public PlayerDigController dig;
    public GameObject dustPrefab;

    [Header("Burst Settings")]
    public float burstInterval = 0.1f;
    public Vector3 localOffset = new Vector3(0, -0.4f, 0);

    private float timer;
    private bool wasPressedLastFrame = false;

    private void Awake()
    {
        if (dig == null)
            dig = GetComponent<PlayerDigController>();
    }

    private void Update()
    {
        if (!GameManager.I.isAlive || dig == null || dustPrefab == null)
            return;

        bool pressed = Input.GetMouseButton(0);

        // --------------------
        // TAP → single burst
        // --------------------
        if (pressed && !wasPressedLastFrame)
        {
            SpawnDust();
        }

        // --------------------
        // HOLD → repeated bursts
        // --------------------
        if (dig.IsHolding())
        {
            timer += Time.deltaTime;

            if (timer >= burstInterval)
            {
                SpawnDust();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }

        wasPressedLastFrame = pressed;
    }

    private void SpawnDust()
    {
        Vector3 pos = transform.TransformPoint(
            localOffset + new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.05f, 0.05f),
                0f
            )
        );

        GameObject fx = Instantiate(dustPrefab, pos, Quaternion.identity);
        Destroy(fx, 2f);
    }
}
