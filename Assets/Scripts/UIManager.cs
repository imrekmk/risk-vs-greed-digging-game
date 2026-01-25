using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CanvasGroup hudGroup;

    public TextMeshProUGUI depthText;
    public TextMeshProUGUI unbankedText;
    public TextMeshProUGUI bankedText;

    public GameObject gameOverPanel;
    public GameObject successPanel;
    public GameObject reviveButton; // optional

    private void OnEnable()
    {
        GameManager.I.onPlayerDied.AddListener(OnDied);
        GameManager.I.onRunBanked.AddListener(OnBanked);
    }

    private void OnDisable()
    {
        if (GameManager.I == null) return;
        GameManager.I.onPlayerDied.RemoveListener(OnDied);
        GameManager.I.onRunBanked.RemoveListener(OnBanked);
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    private void Update()
    {
        float depth = GameManager.I.GetDepth();
        depthText.text = $"Depth: {depth:0}";
        unbankedText.text = $"Run: {GameManager.I.unbankedDiamonds}";
        bankedText.text = $"Bank: {GameManager.I.bankedDiamonds}";
    }

    public void ToggleHUD()
    {
        bool on = hudGroup.alpha > 0.5f;
        hudGroup.alpha = on ? 0f : 1f;
        hudGroup.interactable = !on;
        hudGroup.blocksRaycasts = !on;
    }

    private void OnDied()
    {
        gameOverPanel.SetActive(true);
        successPanel.SetActive(false);

        if (reviveButton != null)
            reviveButton.SetActive(!GameManager.I.reviveUsed);
    }

    private void OnBanked()
    {
        successPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void OnRestart()
    {
        GameManager.I.RestartScene();
    }

    public void OnRevive()
    {
        if (GameManager.I.TryRevive())
        {
            gameOverPanel.SetActive(false);
        }
    }
}
