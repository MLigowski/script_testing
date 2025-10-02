using UnityEngine;
using TMPro;

public class MineralUIManager : MonoBehaviour
{
    public static MineralUIManager Instance;

    public TextMeshProUGUI mineralCountText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateMineralCount(int count)
    {
        mineralCountText.text = "Minera³y: " + count;
    }
}
