using System.Collections.Generic;
using UnityEngine;

public class SideWallManager : MonoBehaviour
{
    public GameConfig config;
    public Transform player;
    public GameObject wallSegmentPrefab;

    private float segmentHeight;

    private readonly Dictionary<int, GameObject> leftSegs = new Dictionary<int, GameObject> ();
    private readonly Dictionary<int, GameObject> rightSegs = new  Dictionary<int, GameObject>();

    private void Start()
    {
        segmentHeight = GetPrefabHeight(wallSegmentPrefab);
        if (segmentHeight <= 0.001f) segmentHeight = 8f;

        Refresh(force: true);
    }

    private void Update()
    {
        if (!GameManager.I.isAlive) return;
        Refresh(force: false);
    }

    private void Refresh(bool force)
    {
        if (config == null || player == null || wallSegmentPrefab == null) return;

        float depth = GameManager.I.GetDepth();

        // IMPORTANT: do not manage walls above the starting area
        if (depth < config.wallSpawnStartDepth)
            return;

        int currentIndex = Mathf.FloorToInt(depth / segmentHeight);

        // Only spawn ahead (below)
        int minIndex = Mathf.FloorToInt(config.wallSpawnStartDepth / segmentHeight); // start index
        int maxIndex = currentIndex + config.wallSegmentsAhead;

        // Spawn required segments
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (!leftSegs.ContainsKey(i)) EnsureSegment(i, true);
            if (!rightSegs.ContainsKey(i)) EnsureSegment(i, false);
        }

        // Cleanup: only destroy segments that are WELL ABOVE the player and also below start area
        float cleanupDepth = depth - (segmentHeight * config.wallCleanupExtra);
        int cleanupIndex = Mathf.FloorToInt(cleanupDepth / segmentHeight);

        CleanupAbove(leftSegs, cleanupIndex, minIndex);
        CleanupAbove(rightSegs, cleanupIndex, minIndex);
    }

    private void EnsureSegment(int index, bool isLeft)
    {
        float yTop = -(index * segmentHeight);
        float yCenter = yTop - (segmentHeight * 0.5f);

        float x = isLeft ? config.wallLeftX : config.wallRightX;
        Vector3 pos = new Vector3(x, yCenter, config.zPlane);

        GameObject go = Instantiate(wallSegmentPrefab, pos, Quaternion.identity, transform);

        if (isLeft) leftSegs[index] = go;
        else rightSegs[index] = go;
    }

    private void CleanupAbove(Dictionary<int, GameObject> dict, int cleanupIndex, int minIndex)
    {
        // Destroy segments with index < cleanupIndex, but never below minIndex (protect start area)
        var keys = new List<int>(dict.Keys);
        foreach (int k in keys)
        {
            if (k < cleanupIndex && k >= minIndex)
            {
                Destroy(dict[k]);
                dict.Remove(k);
            }
        }
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        if (prefab == null) return 0f;
        Renderer r = prefab.GetComponentInChildren<Renderer>();
        if (r == null) return 0f;
        return r.bounds.size.y;
    }
}
