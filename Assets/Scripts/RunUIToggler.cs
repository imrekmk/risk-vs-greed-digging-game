using UnityEngine;
using UnityEngine.UI;

public class RunUIToggler : MonoBehaviour
{
    [Header("UI For Active Run")]
    public GameObject[] runUIObjects;

    [Header("Optional Toggle")]
    public Toggle toggle;

    private void Awake()
    {
        // Auto-hook if toggle assigned
        if (toggle != null)
            toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    // Called by Toggle or manually
    public void OnToggleChanged(bool isOn)
    {
        // isOn = true means "hide UI" (change if you want opposite)
        SetRunUI(!isOn);
    }

    // Enable / Disable gameplay UI
    public void SetRunUI(bool enabled)
    {
        foreach (var obj in runUIObjects)
        {
            if (obj != null)
                obj.SetActive(enabled);
        }
    }

    // Helpers (optional)
    public void ShowRunUI()
    {
        SetRunUI(true);

        if (toggle != null)
            toggle.isOn = false;
    }

    public void HideRunUI()
    {
        SetRunUI(false);

        if (toggle != null)
            toggle.isOn = true;
    }
}
