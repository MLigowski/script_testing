using UnityEngine;

public KeyCode defaultDash = KeyCode.LeftShift;
public KeyCode defaultDig = KeyCode.E;

public class SettingsUI : MonoBehaviour
{
    void Start()
    {
        if (KeybindManager.Instance.DashKey == KeyCode.None)
            KeybindManager.Instance.DashKey = defaultDash;

        if (KeybindManager.Instance.DigKey == KeyCode.None)
            KeybindManager.Instance.DigKey = defaultDig;

        UpdateTexts();
    }

    public void RebindDash()
    {
        KeybindManager.Instance.StartRebinding("Dash");
    }

    public void RebindDig()
    {
        KeybindManager.Instance.StartRebinding("Dig");
    }
}
