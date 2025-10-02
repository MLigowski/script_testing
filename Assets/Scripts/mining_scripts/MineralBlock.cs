using UnityEngine;

public class MineralBlock : MonoBehaviour
{
    public int mineralValue = 1; // ile minera��w dostajesz za zniszczenie tego bloku

    public void BreakBlock()
    {
        // Mo�esz doda� tu animacje, efekty d�wi�kowe itp.

        // Powiadamiamy gracza o zdobyciu minera��w
        PlayerMining.Instance.AddMinerals(mineralValue);

        // Usuwamy blok z gry
        Destroy(gameObject);
    }
}
