using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    public float miningRange = 1.5f;
    public KeyCode mineKey = KeyCode.Tab;
    public float miningCooldown = 1.0f;

    private float lastMineTime = -999f;
    private int mineralsCollected = 0;

    void Awake()
    {
        Instance = this;
        Debug.Log("PlayerMining Instance set");
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
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        int mineralLayerMask = LayerMask.GetMask("Mineral");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, miningRange, mineralLayerMask);

        if (hit.collider != null)
        {
            MineralBlock block = hit.collider.GetComponent<MineralBlock>();
            if (block != null)
            {
                block.BreakBlock();
            }
        }
    }

    public void AddMinerals(int amount)
    {
        mineralsCollected += amount;

        // ✅ teraz przekazujemy też "ile" zebrano w tym momencie
        MineralUIManager.Instance.UpdateMineralCount(mineralsCollected, amount);
    }
}
