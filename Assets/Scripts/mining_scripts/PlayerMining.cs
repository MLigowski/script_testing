﻿using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    [Header("Ustawienia kopania")]
    [Tooltip("Jak daleko gracz może sięgnąć, żeby kopać.")]
    public float miningRange = 1.5f;

    [Tooltip("Ile obrażeń (siły kopania) zadaje jedno uderzenie.")]
    public int miningPower = 1;

    [Tooltip("Klawisz aktywujący kopanie.")]
    public KeyCode mineKey = KeyCode.Tab;

    [Tooltip("Czas w sekundach między kolejnymi kopaniami.")]
    public float miningCooldown = 1.0f;

    private float lastMineTime = -999f;
    private int mineralsCollected = 0;

    // Maski warstw – Mineral i ground
    private int miningLayerMask;

    void Awake()
    {
        Instance = this;
        Debug.Log("✅ PlayerMining Instance ustawione");
    }

    void Start()
    {
        // Ustawiamy maskę z dokładnymi nazwami warstw
        miningLayerMask = LayerMask.GetMask("Mineral", "ground");
        Debug.Log("🎯 Warstwy kopania ustawione: Mineral + ground");
    }

    void Update()
    {
        if (Input.GetKeyDown(mineKey) && Time.time - lastMineTime >= miningCooldown)
        {
            TryMine();
            lastMineTime = Time.time;
        }
    }

    void TryMine()
    {
        // Kierunek kopania zależy od kierunku, w którym zwrócony jest gracz
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Promień debugowy w scenie (czerwony)
        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        // Raycast tylko po warstwach Mineral i ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, miningRange, miningLayerMask);

        if (hit.collider == null)
        {
            Debug.Log("⛏️ Nic nie trafiono!");
            return;
        }

        string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        Debug.Log($"🔸 Trafiono: {hit.collider.name} (Layer: {layerName})");

        // 💎 Minerały
        if (layerName == "Mineral")
        {
            MineralBlock mineral = hit.collider.GetComponent<MineralBlock>();
            if (mineral != null)
            {
                Debug.Log($"💎 Trafiono minerał! (Siła kopania: {miningPower})");
                mineral.BreakBlock(miningPower);
                return;
            }
        }

        // 🪨 Kamienie (layer ground)
        if (layerName == "ground")
        {
            StoneBlock stone = hit.collider.GetComponent<StoneBlock>();
            if (stone != null)
            {
                Debug.Log($"🪨 Trafiono kamień! (Siła kopania: {miningPower})");
                stone.BreakBlock(miningPower);
                return;
            }
        }

        Debug.Log("⚠️ Trafiono obiekt bez skryptu MineralBlock ani StoneBlock!");
    }

    public void AddMinerals(int amount)
    {
        mineralsCollected += amount;

        if (MineralUIManager.Instance != null)
        {
            MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, amount);
        }
        else
        {
            Debug.LogWarning("⚠️ Brak MineralUIManager.Instance w scenie!");
        }
    }
}
