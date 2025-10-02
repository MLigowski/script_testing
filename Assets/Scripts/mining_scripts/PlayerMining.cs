using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public static PlayerMining Instance;

    public float miningRange = 1.5f; // zasiêg kopania
    public KeyCode mineKey = KeyCode.Tab; // klawisz do kopania (mo¿esz zmieniæ na inny)
    public float miningCooldown = 1.0f; // cooldown miêdzy kolejnymi kopaniami (w sekundach)

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
        // Rzucamy raycast w kierunku, w którym patrzy postaæ (w prawo lub w lewo)
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Dodajemy debug, ¿eby zobaczyæ raycast w scenie (czerwony kolor na 1 sekundê)
        Debug.DrawRay(transform.position, direction * miningRange, Color.red, 1f);

        // Raycast z filtrem na warstwê Mineral (upewnij siê, ¿e bloki maj¹ tê warstwê)
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
        MineralUIManager.Instance.UpdateMineralCount(mineralsCollected);
    }
}

