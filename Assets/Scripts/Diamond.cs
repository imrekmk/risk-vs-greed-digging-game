using UnityEngine;

public class Diamond : MonoBehaviour
{
    public int value = 1;

    private bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;

        // Use existing method from GameManager
        GameManager.I.AddUnbanked(value);

        // TODO: optional VFX / sound here

        Destroy(gameObject);
    }
}
