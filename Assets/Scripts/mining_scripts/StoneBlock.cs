using UnityEngine;

public class StoneBlock : MonoBehaviour
{
    [Tooltip("Czas zanim blok zniknie po wykopaniu (dla efekt�w).")]
    public float breakTime = 0.1f;

    public void BreakBlock()
    {
        // Tu mo�esz doda� efekty cz�steczek, d�wi�k, animacj� itp.
        Debug.Log("Zniszczono kamienny blok!");

        // Usuwamy blok z gry
        Destroy(gameObject, breakTime);
    }
}
