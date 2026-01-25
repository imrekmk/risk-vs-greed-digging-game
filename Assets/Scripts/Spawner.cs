using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameConfig config;
    public Transform player;

    public GameObject diamondPrefab;
    public GameObject bombPrefab;
    public GameObject exitPrefab;

    private float nextInteractDepth;
    private float nextExitDepth;
    private bool spawnBombNext = false;   // start with diamond, then bomb, etc.

    private void Start()
    {
        // Starting depth
        float depth = GameManager.I.GetDepth();

        nextInteractDepth = depth + config.firstInteractOffset;
        nextExitDepth = config.exitEveryDepth; // from your existing config
    }

    private void Update()
    {
        if (!GameManager.I.isAlive) return;

        float depth = GameManager.I.GetDepth();

        // ---- Interactables (bombs / diamonds) ----
        while (depth >= nextInteractDepth)
        {
            SpawnInteractable(depth);
            nextInteractDepth += config.interactSpacing;
            spawnBombNext = !spawnBombNext;   // alternate bomb / diamond
        }

        // ---- Exit zones (unchanged idea) ----
        if (depth >= nextExitDepth)
        {
            SpawnExit();
            nextExitDepth += config.exitEveryDepth;
        }
    }

    private void SpawnInteractable(float currentDepth)
    {
        // Spawn ahead of the player along his path
        float y = player.position.y - config.spawnAheadDistance;
        float x = 0f; // single lane in center
        Vector3 pos = new Vector3(x, y, config.zPlane);

        if (spawnBombNext)
        {
            // Spawn a bomb
            Instantiate(bombPrefab, pos, Quaternion.identity);
        }
        else
        {
            // Spawn a diamond slightly higher so it's not buried
            pos.y += config.diamondVerticalOffset;
            Instantiate(diamondPrefab, pos, Quaternion.identity);
        }
    }

    private void SpawnExit()
    {
        float y = player.position.y - config.exitSpawnOffset;
        float x = 0f;
        Vector3 pos = new Vector3(x, y, config.zPlane);

        Instantiate(exitPrefab, pos, Quaternion.identity);
    }
}
