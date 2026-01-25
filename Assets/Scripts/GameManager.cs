using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Refs")]
    public GameConfig config;
    public Transform player;

    [Header("Run State")]
    public int unbankedDiamonds;
    public int bankedDiamonds;

    public bool isAlive = true;
    public bool reviveUsed = false;

    public UnityEvent onRunStart;
    public UnityEvent onPlayerDied;
    public UnityEvent onRunBanked;

    private float startY;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        bankedDiamonds = PlayerPrefs.GetInt("BANKED", 0);
    }

    private void Start()
    {
        startY = player.position.y;
        isAlive = true;
        reviveUsed = false;
        unbankedDiamonds = 0;
        onRunStart?.Invoke();
    }

    public float GetDepth()
    {
        // Depth increases as player goes down (Y becomes more negative)
        return Mathf.Max(0f, startY - player.position.y);
    }

    public float Difficulty01()
    {
        return Mathf.Clamp01(GetDepth() / Mathf.Max(1f, config.depthForMaxDifficulty));
    }

    public void AddUnbanked(int amount)
    {
        unbankedDiamonds += amount;
    }

    public void BankRunAndFinish()
    {
        bankedDiamonds += unbankedDiamonds;
        unbankedDiamonds = 0;

        PlayerPrefs.SetInt("BANKED", bankedDiamonds);
        PlayerPrefs.Save();

        isAlive = false;
        onRunBanked?.Invoke();
    }

    public void KillPlayer()
    {
        if (!isAlive) return;

        isAlive = false;
        unbankedDiamonds = 0; // lose unbanked on death
        onPlayerDied?.Invoke();
    }

    // Day-1 revive (stub). Hook rewarded ad later.
    public bool TryRevive()
    {
        if (reviveUsed) return false;

        reviveUsed = true;
        isAlive = true;
        return true;
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
