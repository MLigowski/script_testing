using UnityEngine;
using TMPro;
using System.Collections;

public class MineralBlock : MonoBehaviour
{
    [Header("W�a�ciwo�ci minera�u")]
    [Tooltip("Ile minera��w gracz otrzymuje po zniszczeniu tego bloku.")]
    public int mineralValue = 1;

    [Header("Wytrzyma�o�� bloku")]
    [Tooltip("Ile uderze� potrzeba, �eby zniszczy� minera�.")]
    public int hitsToBreak = 2;

    private int currentHits = 0;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("UI (opcjonalne)")]
    [Tooltip("Prefab z TextMeshPro 3D, kt�ry pokazuje ile uderze� pozosta�o.")]
    public TextMeshPro worldTextPrefab;

    private TextMeshPro worldTextInstance;

    [Header("Opcje wy�wietlania tekstu")]
    [Tooltip("Offset tekstu nad blokiem.")]
    public Vector3 textOffset = new Vector3(0, 0.6f, -0.01f);
    [Tooltip("Skala prefab TextMeshPro.")]
    public float textScale = 0.5f;
    [Tooltip("Ile sekund tekst ma si� pokaza� je�li nie uderzono bloku ponownie.")]
    public float textDisplayTime = 1.1f;

    private Coroutine hideTextCoroutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        // Tekst pod��a za blokiem
        if (worldTextInstance != null && worldTextInstance.gameObject.activeSelf)
        {
            Vector3 pos = transform.position + textOffset;
            pos.z = -0.01f;
            worldTextInstance.transform.position = pos;
        }
    }

    public void BreakBlock()
    {
        currentHits++;
        int remaining = hitsToBreak - currentHits;

        // Rozja�nienie koloru przy uderzeniu
        if (sr != null)
        {
            float t = Mathf.Clamp01((float)currentHits / hitsToBreak);
            sr.color = Color.Lerp(originalColor, Color.yellow, t * 0.5f);
        }

        // Pokazujemy tekst przy uderzeniu
        ShowHitText(remaining);

        // Zniszczenie bloku po ostatnim uderzeniu
        if (remaining <= 0)
        {
            CollectMinerals();
        }
    }

    private void ShowHitText(int remaining)
    {
        // Je�li nie ma instancji, tworzymy prefab
        if (worldTextInstance == null && worldTextPrefab != null)
        {
            worldTextInstance = Instantiate(worldTextPrefab, transform.position + textOffset, Quaternion.identity);
            worldTextInstance.transform.localScale = Vector3.one * textScale;

            var renderer = worldTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Mineral";
                renderer.sortingOrder = 100;
            }
        }

        // Aktualizujemy tekst
        if (worldTextInstance != null)
        {
            worldTextInstance.text = remaining > 0 ? remaining.ToString() : "";
            worldTextInstance.gameObject.SetActive(true);

            // Restartujemy timer ukrycia tekstu
            if (hideTextCoroutine != null)
                StopCoroutine(hideTextCoroutine);

            hideTextCoroutine = StartCoroutine(HideTextAfterDelay());
        }
    }

    private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(textDisplayTime);

        if (worldTextInstance != null)
            worldTextInstance.gameObject.SetActive(false);

        hideTextCoroutine = null;
    }

    private void CollectMinerals()
    {
        // Dodaj minera�y do gracza
        if (PlayerMining.Instance != null)
        {
            PlayerMining.Instance.AddMinerals(mineralValue);
        }

        // Usu� tekst
        if (worldTextInstance != null)
        {
            Destroy(worldTextInstance.gameObject);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (worldTextInstance != null)
        {
            Destroy(worldTextInstance.gameObject);
        }
    }
}
