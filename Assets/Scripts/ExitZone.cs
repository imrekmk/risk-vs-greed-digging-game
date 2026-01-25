using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private bool used = false;

    // Called when user taps/clicks the exit object (via raycast)
    public void Interact()
    {
        if (used) return;
        used = true;

        GameManager.I.BankRunAndFinish();
    }
}
