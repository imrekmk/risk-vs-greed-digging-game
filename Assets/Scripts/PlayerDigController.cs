using UnityEngine;

public class PlayerDigController : MonoBehaviour
{
    [Header("Ground to move down while digging")]
    public Transform centerGround;      // GroundCenter
    public GameObject centerGrass;      // GrassCenter (optional)

    [Header("Stop digging when interactable is ahead")]
    public float interactCheckDistance = 3f; // how far below to look
    public float interactCheckRadius = 1f;   // tunnel width radius

    private GameConfig cfg;
    private bool holding;
    private bool startedDiggingOnce = false;
    private Animator anim;

    [Header("Input")]
    public float holdThreshold = 0.12f; // seconds to count as "hold"
    private float pressTime = -1f;
    private bool isHold = false;


    private void Start()
    {
        cfg = GameManager.I.config;
        anim = GetComponentInChildren<Animator>();
    }

    private bool wasHolding = false;

    private void Update()
    {
        if (!GameManager.I.isAlive) return;

        bool canDigNow = CanDig();

        // ----- press started (tap begins) -----
        if (Input.GetMouseButtonDown(0))
        {
            pressTime = Time.time;
            isHold = false;

            // Tap should only trigger the one-shot dig animation
            if (anim != null && canDigNow)
                anim.SetTrigger("DigTap");

            // First interaction removes grass cap
            if (!startedDiggingOnce)
            {
                startedDiggingOnce = true;
                if (centerGrass != null) Destroy(centerGrass);
            }
        }

        // ----- while pressed: decide if this becomes a hold -----
        if (Input.GetMouseButton(0))
        {
            if (!isHold && pressTime > 0f && (Time.time - pressTime) >= holdThreshold)
                isHold = true;
        }

        // ----- released -----
        if (Input.GetMouseButtonUp(0))
        {
            pressTime = -1f;
            isHold = false;
        }

        // HOLD drives looping dig animation
        if (anim != null)
            anim.SetBool("IsDigging", isHold && canDigNow);

        // Dig movement only when holding (not on tap)
        holding = isHold;
    }
    private bool CanDig()
    {
        if (IsInteractableAhead())
            return false;

        // If BombGrabThrow disabled digging, respect that
        if (!enabled)
            return false;

        return true;
    }

    private void FixedUpdate()
    {
        if (!GameManager.I.isAlive) return;
        if (!holding) return;
        if (centerGround == null) return;

        // If a bomb or diamond is just ahead → stop digging
        if (IsInteractableAhead())
            return;

        // Move the middle ground down (player is child, so he goes with it)
        float speed = cfg.digBoostSpeed; // digging speed
        Vector3 pos = centerGround.position;
        pos += Vector3.down * speed * Time.fixedDeltaTime;
        centerGround.position = pos;
    }

    private bool IsInteractableAhead()
    {
        // Look a bit below the player in a small radius
        Vector3 checkCenter = transform.position + Vector3.down * interactCheckDistance;

        Collider[] hits = Physics.OverlapSphere(checkCenter, interactCheckRadius);
        foreach (var h in hits)
        {
            if (h == null) continue;
            if (h.CompareTag("Bomb") || h.CompareTag("Diamond"))
                return true;
        }
        return false;
    }

    public bool IsHolding() => holding;
}
