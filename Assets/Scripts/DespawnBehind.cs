using UnityEngine;

public class DespawnBehind : MonoBehaviour
{
    private Transform player;
    private GameConfig cfg;

    private void Start()
    {
        player = GameManager.I.player;
        cfg = GameManager.I.config;
    }

    private void Update()
    {
        if (player == null) return;

        // If object is too far above the player (behind), destroy it
        if (transform.position.y > player.position.y + cfg.despawnBehindDistance)
        {
            Destroy(gameObject);
        }
    }
}
