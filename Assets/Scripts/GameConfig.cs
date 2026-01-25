using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("World / Plane")]
    public float zPlane = 0f;
    public float xSpawnRange = 1.5f;   // bombs/diamonds spawn within this X range

    [Header("Digging")]
    public float baseDownSpeed = 2.5f;
    public float digBoostSpeed = 4.0f;

    [Header("Spawning")]
    public float spawnAheadDistance = 14f;
    public float despawnBehindDistance = 18f;

    public float baseSpawnInterval = 0.7f;
    public float minSpawnInterval = 0.3f;

    [Range(0f, 1f)] public float bombChanceAtStart = 0.25f;
    [Range(0f, 1f)] public float bombChanceAtDeep = 0.55f;

    [Header("Difficulty (by depth)")]
    public float depthForMaxDifficulty = 250f;

    [Header("Bomb Timers")]
    public float bombTimerStart = 3.0f;
    public float bombTimerMin = 1.2f;

    [Header("Throwing")]
    public float dragMaxDistanceWorld = 2.0f;   // how far bomb can be dragged from player while holding
    public float throwPower = 12f;
    public float upwardBias = 0.35f;            // makes throws feel more upward by default

    [Header("Exit")]
    public float exitEveryDepth = 60f;          // spawn an Exit roughly every X depth
    public float exitSpawnOffset = 10f;         // how far ahead to spawn exit

    [Header("Explosion")]
    public float explosionRadius = 1.8f;

    [Header("Tunnel")]
    public float segmentHeight = 8f;
    public int segmentsAhead = 3;
    public int segmentsBehind = 2;

    [Header("Diamonds")]
    public float diamondVerticalOffset = 0.3f;   // tweak this value

    [Header("Interactable Spawning")]
    public float interactSpacing = 6f;        // distance between bomb/diamond rows (tweak)
    public float firstInteractOffset = 4f;    // how far after start the first spawn appears

    [Header("Side Walls")]
    public float wallSegmentHeight = 8f;
    public float wallLeftX = -3f;
    public float wallRightX = 3f;
    public int wallSegmentsAhead = 6;
    public int wallSegmentsBehind = 2;
    public float wallSpawnStartDepth = 4f;  // do not manage walls above this depth
    public float wallCleanupExtra = 2f;     // buffer before destroying above

}
