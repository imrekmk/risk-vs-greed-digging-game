using UnityEngine;
using TMPro;

public class Bomb : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private Rigidbody rb;
    private GameConfig cfg;

    private float timer;
    private bool timerStarted = false;
    private bool exploded = false;
    public GameObject explosionVFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cfg = GameManager.I.config;

        // Freeze bomb until thrown
        rb.isKinematic = true;
        rb.useGravity = false;

        float diff = GameManager.I.Difficulty01();
        timer = Mathf.Lerp(cfg.bombTimerStart, cfg.bombTimerMin, diff);

        if (timerText != null)
            timerText.text = "";


    }

    private void Update()
    {
        if (!timerStarted || exploded || !GameManager.I.isAlive) return;

        timer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0f)
            Explode();
    }

    public void StartTimer()
    {
        timerStarted = true;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timer).ToString();
    }

    public void EnablePhysics()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Spawn explosion VFX
        if (explosionVFX != null)
        {
            GameObject fx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(fx, 5f); // delete after 5 seconds
        }

        // Damage check
        Collider[] hits = Physics.OverlapSphere(transform.position, cfg.explosionRadius);
        foreach (var h in hits)
        {
            if (h.CompareTag("Player"))
            {
                GameManager.I.KillPlayer();
            }
        }

        // Destroy bomb object
        Destroy(gameObject);
    }

}
