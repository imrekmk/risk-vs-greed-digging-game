using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameConfig config;
    public Transform player;

    [Header("Diamond Variants")]
    public GameObject[] diamondPrefabs;

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
            pos.y += config.diamondVerticalOffset;

            // Safety check
            if (diamondPrefabs == null || diamondPrefabs.Length == 0)
            {
                Debug.LogWarning("No diamond prefabs assigned!");
                return;
            }

            // Pick random diamond
            int index = Random.Range(0, diamondPrefabs.Length);
            GameObject chosenDiamond = diamondPrefabs[index];

            Instantiate(chosenDiamond, pos, Quaternion.identity);

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
