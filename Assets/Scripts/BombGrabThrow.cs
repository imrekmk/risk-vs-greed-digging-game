using UnityEngine;

public class BombGrabThrow : MonoBehaviour
{
    public Camera cam;
    public Transform bombHoldPoint;

    private enum State { Idle, HoldingBomb, AimingBomb }
    private State state = State.Idle;

    private Bomb heldBomb;
    private Rigidbody heldRb;
    private Vector3 dragStartWorld;

    private GameConfig cfg;
    private PlayerDigController digController;
    private Animator anim;

    private void Start()
    {
        cfg = GameManager.I.config;
        digController = GetComponent<PlayerDigController>();

        if (cam == null)
            cam = Camera.main;
        anim = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        if (!GameManager.I.isAlive) return;

        switch (state)
        {
            case State.Idle:
                HandleIdleInput();
                break;

            case State.HoldingBomb:
                HoldBombAtHand();
                HandleHoldingBombInput();
                break;

            case State.AimingBomb:
                HoldBombAtHand();
                HandleAimingBombInput();
                break;
        }
    }

    // --------------------------------------------------------------------
    // IDLE STATE  → Tap to interact with Bomb or Diamond
    // --------------------------------------------------------------------
    private void HandleIdleInput()
    {
        if (Input.GetMouseButtonDown(0))
            TryInteractWithObject();
    }

    private void TryInteractWithObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;

        // ---------------------------
        // Bomb pick
        // ---------------------------
        if (hit.collider.CompareTag("Bomb"))
        {
            TryPickBomb(hit.collider.GetComponent<Bomb>());
            return;
        }

        // ---------------------------
        // Diamond collect
        // ---------------------------
        if (hit.collider.CompareTag("Diamond"))
        {
            TryPickDiamond(hit.collider.GetComponent<Diamond>());
            return;
        }

        // ---------------------------
        // Exit collect
        // ---------------------------

        if (hit.collider.CompareTag("Exit"))
        {
            ExitZone exit = hit.collider.GetComponent<ExitZone>();
            if (exit != null)
                exit.Interact();
            return;
        }

    }

    // --------------------------------------------------------------------
    // PICK BOMB
    // --------------------------------------------------------------------
    private void TryPickBomb(Bomb bomb)
    {
        if (bomb == null) return;

        heldBomb = bomb;
        heldRb = bomb.GetComponent<Rigidbody>();

        // Stop digging immediately
        if (digController != null)
            digController.enabled = false;

        // Freeze bomb + snap to hand
        heldRb.isKinematic = true;
        heldRb.useGravity = false;

        bomb.transform.SetParent(bombHoldPoint);
        bomb.transform.localPosition = Vector3.zero;
        bomb.transform.localRotation = Quaternion.identity;

        state = State.HoldingBomb;
    }

    // --------------------------------------------------------------------
    // HOLDING BOMB
    // --------------------------------------------------------------------
    private void HandleHoldingBombInput()
    {
        // Tap again to start aiming
        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorld = ScreenToWorldOnPlane(Input.mousePosition);
            state = State.AimingBomb;
        }
    }

    private void HoldBombAtHand()
    {
        if (heldBomb == null) return;
        heldBomb.transform.position = bombHoldPoint.position;
    }

    // --------------------------------------------------------------------
    // AIMING → Throw when finger/mouse released
    // --------------------------------------------------------------------
    private void HandleAimingBombInput()
    {
        if (Input.GetMouseButtonUp(0))
            ReleaseThrow();
    }

    private void ReleaseThrow()
    {
        if (heldBomb == null || heldRb == null)
        {
            ResetToIdle();
            return;
        }

        // play throw anim
        if (anim != null)
            anim.SetTrigger("Throw");

        Vector3 releaseWorld = ScreenToWorldOnPlane(Input.mousePosition);
        Vector3 pull = dragStartWorld - releaseWorld;
        pull.z = 0f;

        // Upward help
        pull.y = Mathf.Abs(pull.y) + cfg.upwardBias;

        Vector3 dir = pull.sqrMagnitude > 0.001f ? pull.normalized : Vector3.up;
        float power = cfg.throwPower * Mathf.Clamp(pull.magnitude, 0.2f, 2.5f);

        // Detach from hand
        heldBomb.transform.SetParent(null);

        // Enable physics
        heldRb.isKinematic = false;
        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;

        // Throw
        heldRb.AddForce(dir * power, ForceMode.Impulse);

        // Clear
        heldBomb = null;
        heldRb = null;

        ResetToIdle();
    }

    private void ResetToIdle()
    {
        state = State.Idle;

        // Resume digging
        if (digController != null)
            digController.enabled = true;
    }

    // --------------------------------------------------------------------
    // DIAMOND COLLECTION
    // --------------------------------------------------------------------
    private void TryPickDiamond(Diamond diamond)
    {
        if (diamond == null) return;

        // Stop digging
        if (digController != null)
            digController.enabled = false;

        // Collect diamond
        diamond.Collect();

        // Resume digging after a short delay
        Invoke(nameof(ResumeDigging), 0.1f);
    }

    private void ResumeDigging()
    {
        if (digController != null)
            digController.enabled = true;
    }

    // --------------------------------------------------------------------
    // Utility
    // --------------------------------------------------------------------
    private Vector3 ScreenToWorldOnPlane(Vector3 screenPos)
    {
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }
}
